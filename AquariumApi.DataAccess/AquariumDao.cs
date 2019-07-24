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
        void SetAquariumDevice(int aquariumId,int deviceId);
        AquariumDevice AddAquariumDevice(AquariumDevice device);
        AquariumDevice UpdateAquariumDevice(AquariumDevice device);
        AquariumDevice DeleteAquariumDevice(int deviceId);
        AquariumDevice GetAquariumDeviceById(int deviceId);
        AquariumDevice ApplyAquariumDeviceHardware(int deviceId, AquariumDevice updatedDevice);
        List<AquariumPhoto> GetAquariumPhotos(int aquariumId);
        void DeleteAquariumPhoto(int photoId);
        AquariumPhoto GetAquariumPhotoById(int photoId);
        AquariumPhoto AddAquariumPhoto(AquariumPhoto photo);
        AquariumDevice GetAquariumDeviceByIpAndKey(string ipAddress,string deviceKey);
        List<AquariumOverviewResponse> GetAquariumOverviews();
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
        /* Aquarium */
        public List<Aquarium> GetAquariums()
        {
            return _dbAquariumContext.TblAquarium.AsNoTracking()
                .Include(aq => aq.Device)
                .ToList();
        }
        public List<AquariumOverviewResponse> GetAquariumOverviews()
        {
            var aquariums = from d in _dbAquariumContext.TblAquarium
                    select new AquariumOverviewResponse
                    {
                        FishCount = d.Fish.Count(),
                        FeedingCount = d.Feedings.Count(),
                        HasDevice = (d.Device != null),
                    };
            return aquariums.ToList();
        }
        public Aquarium GetAquariumById(int aquariumId)
        {
            var aquarium = _dbAquariumContext.TblAquarium.AsNoTracking()
                .Where(aq => aq.Id == aquariumId)
                .Include(aq => aq.Fish)
                .Include(aq => aq.Feedings)
                .Include(aq => aq.Device).ThenInclude(d => d.CameraConfiguration)
                .First();

            if (aquarium.Device != null && aquarium.Device.CameraConfiguration == null)
                aquarium.Device.CameraConfiguration = new CameraConfiguration();
            return aquarium;
        }
        public Aquarium AddAquarium(Aquarium model)
        {
            _dbAquariumContext.TblAquarium.Add(model);
            _dbAquariumContext.SaveChanges();
            return model;
        }
        public Aquarium UpdateAquarium(Aquarium aquarium)
        {
            var aqToUpdate = _dbAquariumContext.TblAquarium.SingleOrDefault(aq => aq.Id == aquarium.Id);
            if (aqToUpdate == null)
                throw new KeyNotFoundException();

            aqToUpdate = _mapper.Map(aquarium, aqToUpdate);

            _dbAquariumContext.SaveChanges();
            return GetAquariumById(aqToUpdate.Id);
        }
        public void DeleteAquarium(int aquariumId)
        {
            var aquarium = _dbAquariumContext.TblAquarium.Where(aq => aq.Id == aquariumId).Include(aq => aq.Device).Include(aq => aq.Fish).First();
            _dbAquariumContext.TblSnapshot.RemoveRange(_dbAquariumContext.TblSnapshot.Where(aq => aq.AquariumId == aquariumId));
            if(aquarium.Device != null)
                _dbAquariumContext.TblDevice.Remove(aquarium.Device);
            _dbAquariumContext.TblAquarium.Remove(aquarium);
            _dbAquariumContext.SaveChanges();
        }

        /* Aquarium Snapshots */
        public AquariumSnapshot AddSnapshot(AquariumSnapshot model)
        {
            _dbAquariumContext.TblSnapshot.Add(model);
            _dbAquariumContext.SaveChanges();
            return model;
        }
        public List<AquariumSnapshot> GetSnapshots()
        {
            return _dbAquariumContext.TblSnapshot.Include(s => s.Photo).AsNoTracking().ToList();
        }
        public AquariumSnapshot GetSnapshotById(int id)
        {
            return _dbAquariumContext.TblSnapshot.AsNoTracking().Include(s => s.Photo).Where(snap => snap.Id == id).First();
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
        public AquariumSnapshot DeleteSnapshot(int snapshotId)
        {
            var snapshot = _dbAquariumContext.TblSnapshot.SingleOrDefault(e => e.Id == snapshotId);
            if (snapshot == null)
                throw new KeyNotFoundException();
            _dbAquariumContext.TblSnapshot.Remove(snapshot);
            _dbAquariumContext.SaveChanges();
            if (snapshot.PhotoId != null)
                DeleteAquariumPhoto(snapshot.PhotoId.Value);
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

        public AquariumDevice AddAquariumDevice(AquariumDevice device)
        {
            _dbAquariumContext.TblDevice.Add(device);
            if (device.CameraConfiguration == null) device.CameraConfiguration = new CameraConfiguration();
            _dbAquariumContext.TblCameraConfiguration.Add(device.CameraConfiguration);
            _dbAquariumContext.SaveChanges();
            return device;
        }
        public AquariumDevice GetAquariumDeviceById(int deviceId)
        {
            var device = _dbAquariumContext.TblDevice.AsNoTracking()
                .Where(s => s.Id == deviceId)
                .Include(e => e.CameraConfiguration)
                .First();
            if (device.CameraConfiguration == null)
                device.CameraConfiguration = new CameraConfiguration();
            return device;
        }
        public AquariumDevice DeleteAquariumDevice(int deviceId)
        {
            var device = _dbAquariumContext.TblDevice
                .Include(e => e.CameraConfiguration)
                .SingleOrDefault(d => d.Id == deviceId);
            if (device == null)
                throw new KeyNotFoundException();
            _dbAquariumContext.TblDevice.Remove(device);
            if (device.CameraConfiguration != null)
                _dbAquariumContext.TblCameraConfiguration.Remove(device.CameraConfiguration);
            _dbAquariumContext.SaveChanges();
            return device;
        }
        public AquariumDevice UpdateAquariumDevice(AquariumDevice device)
        {
            var deviceToUpdate = _dbAquariumContext.TblDevice
                .Include(d => d.CameraConfiguration)
                .SingleOrDefault(s => s.Id == device.Id);
            if(deviceToUpdate == null)
                throw new KeyNotFoundException();

             _mapper.Map(device, deviceToUpdate);

            //Also map modules
            if(device.CameraConfiguration != null)
                _mapper.Map(device.CameraConfiguration, deviceToUpdate.CameraConfiguration);

            _dbAquariumContext.SaveChanges();
            return GetAquariumDeviceById(deviceToUpdate.Id);
        }
        public void SetAquariumDevice(int aquariumId, int deviceId)
        {
            var device = _dbAquariumContext.TblDevice.Where(s => s.Id == deviceId).First();
            if (device == null)
                throw new KeyNotFoundException();
            var aquarium = _dbAquariumContext.TblAquarium.Where(s => s.Id == aquariumId).First();
            if (aquarium == null)
                throw new KeyNotFoundException();

            device.AquariumId = aquarium.Id;
            _dbAquariumContext.TblDevice.Update(device);
            _dbAquariumContext.SaveChanges();
        }

        public AquariumDevice ApplyAquariumDeviceHardware(int deviceId, AquariumDevice updatedDevice)
        {
            var d = GetAquariumDeviceById(deviceId);
            d.EnabledPhoto = updatedDevice.EnabledPhoto;
            d.EnabledTemperature = updatedDevice.EnabledTemperature;
            d.EnabledLighting = updatedDevice.EnabledLighting;
            d.EnabledPh = updatedDevice.EnabledPh;
            d.EnabledNitrate = updatedDevice.EnabledNitrate;
            return UpdateAquariumDevice(d);
        }

        public AquariumPhoto GetAquariumPhotoById(int photoId)
        {
            return _dbAquariumContext.TblAquariumPhoto.AsNoTracking()
                .Where(p => p.Id == photoId)
                .First();
        }
        public List<AquariumPhoto> GetAquariumPhotos(int aquariumId)
        {
            return _dbAquariumContext.TblAquariumPhoto.AsNoTracking()
                .Where(p => p.AquariumId == aquariumId)
                .ToList();
        }
        public AquariumPhoto AddAquariumPhoto(AquariumPhoto photo)
        {
            _dbAquariumContext.TblAquariumPhoto.Add(photo);
            _dbAquariumContext.SaveChanges();
            return photo;
        }
        public void DeleteAquariumPhoto(int photoId)
        {
            var photo = GetAquariumPhotoById(photoId);
            if (File.Exists(photo.Filepath))
                File.Delete(photo.Filepath);
            _dbAquariumContext.TblAquariumPhoto.Remove(photo);
            _dbAquariumContext.SaveChanges();
        }

        public AquariumDevice GetAquariumDeviceByIpAndKey(string ipAddress,string deviceKey)
        {
            if (ipAddress == "::1") ipAddress = "localhost";
            return _dbAquariumContext.TblDevice.AsNoTracking()
                .Where(s => s.Address == ipAddress && s.PrivateKey == deviceKey)
                .Include(e => e.Aquarium)
                .Include(e => e.CameraConfiguration)
                .First();
        }

    }
}
