using AquariumApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using AutoMapper;

namespace AquariumApi.DataAccess
{
    public interface IAquariumDao
    {
        Aquarium AddAquarium(Aquarium aquarium);
        List<Aquarium> GetAquariums();
        Aquarium GetAquariumById(int aquariumId);
        Aquarium UpdateAquarium(Aquarium aquarium);
        void DeleteAquarium(int aquariumId);

        AquariumSnapshot AddSnapshot(AquariumSnapshot snapshot);
        List<AquariumSnapshot> GetSnapshots();
        AquariumSnapshot GetSnapshotById(int snapshotId);
        AquariumSnapshot DeleteSnapshot(int snapshotId);

        Species AddSpecies(Species species);
        List<Species> GetAllSpecies();
        Species UpdateSpecies(Species species);
        void DeleteSpecies(int speciesId);

        Fish AddFish(Fish fish);
        Fish GetFishById(int fishId);
        Fish UpdateFish(Fish fish);
        void DeleteFish(int fishId);

        Feeding AddFeeding(Feeding feeding);
        Feeding GetFeedingById(int feedingId);
        List<Feeding> GetFeedingByAquariumId(int aquariumId);
        Feeding UpdateFeeding(Feeding feeding);
        void DeleteFeeding(int feedingId);
        Species GetSpeciesById(int speciesId);
    }

    public class AquariumDao : IAquariumDao
    {
        private readonly DbAquariumContext _dbAquariumContext;
        private readonly ILogger<AquariumDao> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AquariumDao(IMapper mapper,IConfiguration config,DbAquariumContext dbAquariumContext, ILogger<AquariumDao> logger)
        {
            _mapper = mapper;
            _config = config;
            _dbAquariumContext = dbAquariumContext;
            _logger = logger;
        }

        public List<Aquarium> GetAquariums()
        {
            return _dbAquariumContext.TblAquarium.AsNoTracking().ToList();
        }
        public Aquarium GetAquariumById(int aquariumId)
        {
            var a = _dbAquariumContext.TblFish.Where(f => f.AquariumId == aquariumId).ToList();

            var aquarium = _dbAquariumContext.TblAquarium
                .Where(aq => aq.Id == aquariumId)
                .Include(aq => aq.CameraConfiguration)
                .Include(aq => aq.Fish)
                .Include(aq => aq.Feedings)
                .First();
            return aquarium;
        }
        public List<AquariumSnapshot> GetSnapshots()
        {
            return _dbAquariumContext.TblSnapshot.AsNoTracking().ToList();
        }
        public AquariumSnapshot GetSnapshotById(int id)
        {
            return _dbAquariumContext.TblSnapshot.AsNoTracking().Where(snap => snap.Id == id).First();
        }

        public Aquarium AddAquarium(Aquarium model)
        {
            _dbAquariumContext.TblAquarium.Add(model);
            _dbAquariumContext.SaveChanges();
            return model;
        }
        public void DeleteAquarium(int aquariumId)
        {
            var aquarium = _dbAquariumContext.TblAquarium.Where(aq => aq.Id == aquariumId).Include(aq => aq.CameraConfiguration).Include(aq => aq.Fish).First();
            _dbAquariumContext.TblSnapshot.RemoveRange(_dbAquariumContext.TblSnapshot.Where(aq => aq.AquariumId == aquariumId));
            if(aquarium.CameraConfiguration != null)
                _dbAquariumContext.TblCameraConfiguration.Remove(aquarium.CameraConfiguration);
            _dbAquariumContext.TblAquarium.Remove(aquarium);
            _dbAquariumContext.SaveChanges();
        }
        public List<AquariumSnapshot> DeleteSnapshotsByAquarium(int aquariumId)
        {
            var snapshots = GetSnapshots().Where(snap => snap.AquariumId == aquariumId);
            _dbAquariumContext.TblSnapshot.RemoveRange(snapshots);
            _dbAquariumContext.SaveChanges();

            var photoPath = _config["PhotoSubPath"] + aquariumId;
            //Check if photo folder exists
            if (Directory.Exists(photoPath))
                Directory.Delete(photoPath,true);
            return snapshots.ToList();

        }

        public AquariumSnapshot AddSnapshot(AquariumSnapshot model)
        {
            _dbAquariumContext.TblSnapshot.Add(model);
            _dbAquariumContext.SaveChanges();
            return model;
        }

        public Aquarium UpdateAquarium(Aquarium aquarium)
        {
            var aqToUpdate = _dbAquariumContext.TblAquarium.Include(aq => aq.CameraConfiguration).SingleOrDefault(aq => aq.Id == aquarium.Id);
            if (aqToUpdate == null)
                throw new KeyNotFoundException();

            if (aquarium.CameraConfiguration == null)
                aquarium.CameraConfiguration = _mapper.Map<CameraConfiguration>(aqToUpdate.CameraConfiguration) ?? new CameraConfiguration();
            else if(aqToUpdate.CameraConfiguration != null)
                aquarium.CameraConfiguration.Id = aqToUpdate.CameraConfiguration.Id;
            aqToUpdate = _mapper.Map(aquarium, aqToUpdate);

            _dbAquariumContext.SaveChanges();
            return aqToUpdate;
        }

        public AquariumSnapshot DeleteSnapshot(int snapshotId)
        {
            var snapshot = GetSnapshots().Where(snap => snap.Id == snapshotId).First();
            _dbAquariumContext.TblSnapshot.Remove(snapshot);
            _dbAquariumContext.SaveChanges();

            //Check if file exists
            if (File.Exists(snapshot.PhotoPath))
                System.IO.File.Delete(snapshot.PhotoPath);
            return snapshot;
        }

        

        /* Species */
        public Species AddSpecies(Species species)
        {
            _dbAquariumContext.TblSpecies.Add(species);
            _dbAquariumContext.SaveChanges();
            _dbAquariumContext.Entry(species).State = EntityState.Detached;
            return species;
        }
        public List<Species> GetAllSpecies()
        {
            var species = _dbAquariumContext.TblSpecies.AsNoTracking().ToList();
            species.ForEach(s =>
            {
                var fish = _dbAquariumContext.TblFish.AsNoTracking().Where(f => f.SpeciesId == s.Id);
                s.FishCount = fish.Count();
                s.AquariumCount = fish.Select(f => f.AquariumId).Distinct().Count();
            });
            return species;
        }
        public Species GetSpeciesById(int speciesId)
        {
            var species = _dbAquariumContext.TblSpecies.AsNoTracking().Where(s => s.Id == speciesId).First();
            return species;
        }
        public Species UpdateSpecies(Species species)
        {
            var speciesTpUpdate = _dbAquariumContext.TblSpecies.AsNoTracking().Where(s => s.Id == species.Id).First();
            if (species == null)
                throw new KeyNotFoundException();
            _dbAquariumContext.TblSpecies.Update(species);
            _dbAquariumContext.SaveChanges();
            return species;
        }

        public void DeleteSpecies(int speciesId)
        {
            var speciesTpUpdate = _dbAquariumContext.TblSpecies.Where(s => s.Id == speciesId).First();
            if (speciesTpUpdate == null)
                throw new KeyNotFoundException();
            _dbAquariumContext.TblSpecies.Remove(speciesTpUpdate);
            _dbAquariumContext.SaveChanges();
        }

        /* Fish */
        public Fish GetFishById(int fishId)
        {
            return _dbAquariumContext.TblFish.AsNoTracking()
                .Include(f => f.Species)
                .Include(f => f.Aquarium)
                .Include(f => f.Feedings)
                .Where(s => s.Id == fishId).First();
        }
        public Fish AddFish(Fish fish)
        {
            fish.Species = null;
            fish.Aquarium = null; //todo separate this into a request/response models
            _dbAquariumContext.TblFish.Add(fish);
            _dbAquariumContext.SaveChanges();
            return fish;
        }
        public Fish UpdateFish(Fish fish)
        {
            var fishToUpdate = _dbAquariumContext.TblFish.Where(s => s.Id == fish.Id).First();
            if (fishToUpdate == null)
                throw new KeyNotFoundException();

            fishToUpdate.Name = fish.Name;
            fishToUpdate.Date = fish.Date;
            fishToUpdate.Gender = fish.Gender;
            fishToUpdate.Description = fish.Description;
            fishToUpdate.SpeciesId = fish.SpeciesId;
            //fishToUpdate.AquariumId = fish.AquariumId; //todo automapper
            _dbAquariumContext.TblFish.Update(fishToUpdate);
            _dbAquariumContext.SaveChanges();
            return fish;
        }

        public void DeleteFish(int fishId)
        {
            var fishToUpdate = _dbAquariumContext.TblFish.Where(s => s.Id == fishId).First();
            if (fishToUpdate == null)
                throw new KeyNotFoundException();
            _dbAquariumContext.TblFish.Remove(fishToUpdate);
            _dbAquariumContext.SaveChanges();
        }

        /* Feeding */
        public Feeding AddFeeding(Feeding feeding)
        {
            feeding.Fish = null;
            feeding.Aquarium = null; //todo separate this into a request/response models
            _dbAquariumContext.TblFeeding.Add(feeding);
            _dbAquariumContext.SaveChanges();
            return feeding;
        }
        public Feeding GetFeedingById(int feedingId)
        {
            return _dbAquariumContext.TblFeeding.AsNoTracking()
                .Include(f => f.Fish)
                .Include(f => f.Aquarium)
                .Where(s => s.Id == feedingId).First();
        }
        public List<Feeding> GetFeedingByAquariumId(int aquariumId)
        {
            return _dbAquariumContext.TblFeeding.AsNoTracking().Where(f => f.AquariumId == aquariumId).ToList();
        }
        public Feeding UpdateFeeding(Feeding feeding)
        {
            var feedingToUpdate = _dbAquariumContext.TblFeeding.Where(s => s.Id == feeding.Id).First();
            if (feedingToUpdate == null)
                throw new KeyNotFoundException();

            feedingToUpdate.Date = feeding.Date;
            feedingToUpdate.FishId = feeding.FishId;
            feedingToUpdate.Amount = feeding.Amount;
            feedingToUpdate.FoodBrand = feeding.FoodBrand;
            feedingToUpdate.FoodBrand = feeding.FoodBrand;
            //feedingToUpdate.AquariumId = feeding.AquariumId; //todo automapper
            _dbAquariumContext.TblFeeding.Update(feedingToUpdate);
            _dbAquariumContext.SaveChanges();
            return feeding;
        }

        public void DeleteFeeding(int feedingId)
        {
            var feedingToUpdate = _dbAquariumContext.TblFeeding.Where(s => s.Id == feedingId).First();
            if (feedingToUpdate == null)
                throw new KeyNotFoundException();
            _dbAquariumContext.TblFeeding.Remove(feedingToUpdate);
            _dbAquariumContext.SaveChanges();
        }

        
    }
}
