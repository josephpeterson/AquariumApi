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
        List<Aquarium> GetAquariums();
        List<AquariumSnapshot> GetSnapshots();
        Aquarium AddAquarium(Aquarium aquarium);
        AquariumSnapshot AddSnapshot(AquariumSnapshot snapshot);
        Aquarium UpdateAquarium(Aquarium aquarium);
        void DeleteAquarium(int aquariumId);
        AquariumSnapshot DeleteSnapshot(int snapshotId);
        Aquarium GetAquariumById(int aquariumId);
        AquariumSnapshot GetSnapshotById(int snapshotId);
        List<Species> GetAllSpecies();
        Species UpdateSpecies(Species species);
        void DeleteSpecies(int speciesId);
        Species AddSpecies(Species species);
        Fish AddFish(Fish fish);
        Fish UpdateFish(Fish fish);
        void DeleteFish(int fishId);
        Fish GetFishById(int fishId);
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
            var aquarium = _dbAquariumContext.TblAquarium
                .Where(aq => aq.Id == aquariumId)
                .Include(aq => aq.CameraConfiguration)
                .Include(aq => aq.Fish)
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
            var aquarium = _dbAquariumContext.TblAquarium.Where(aq => aq.Id == aquariumId).Include(aq => aq.CameraConfiguration).First();
            _dbAquariumContext.TblSnapshot.RemoveRange(_dbAquariumContext.TblSnapshot.Where(aq => aq.AquariumId == aquariumId));
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

        public List<Species> GetAllSpecies()
        {
            return _dbAquariumContext.TblSpecies.AsNoTracking().ToList();
        }

        /* Species */
        public Species AddSpecies(Species species)
        {
            _dbAquariumContext.TblSpecies.Add(species);
            _dbAquariumContext.SaveChanges();
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
                .Where(s => s.Id == fishId).First();
        }
        public Fish AddFish(Fish fish)
        {
            _dbAquariumContext.TblFish.Add(fish);
            _dbAquariumContext.SaveChanges();
            return fish;
        }
        public Fish UpdateFish(Fish fish)
        {
            var fishToUpdate = GetFishById(fish.Id);
            if (fishToUpdate == null)
                throw new KeyNotFoundException();
            _dbAquariumContext.TblFish.Update(fish);
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
    }
}
