using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IAquariumService
    {
        Aquarium AddAquarium(Aquarium aquarium);
        List<Aquarium> GetAllAquariums();
        Aquarium GetAquariumById(int id);
        Aquarium UpdateAquarium(Aquarium aquarium);
        void DeleteAquarium(int aquariumId);

        AquariumSnapshot TakeSnapshot(int aquariumId,bool takePhoto);
        List<AquariumSnapshot> GetSnapshots(int aquariumId);
        AquariumSnapshot GetSnapshotById(int snapshotId);
        void DeleteSnapshot(int removeSnapshotId);

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

        AquariumDevice AddAquariumDevice(AquariumDevice device);
        AquariumDevice GetAquariumDeviceById(int deviceId);
        AquariumDevice DeleteAquariumDevice(int deviceId);
        AquariumDevice UpdateAquariumDevice(AquariumDevice deviceId);
        void SetAquariumDevice(int aquariumId,int deviceId);
        AquariumDevice ApplyAquariumDeviceHardware(int deviceId, AquariumDevice updatedDevice);
        AquariumPhoto GetAquariumPhotoById(int photoId);
        AquariumPhoto AddAquariumPhoto(AquariumPhoto photo);
        List<AquariumPhoto> GetAquariumPhotos(int aquariumId);
        AquariumDevice GetAquariumDeviceByIpAndKey(string ipAddress,string deviceKey);
        AquariumDevice UpdateDeviceCameraConfiguration(CameraConfiguration config);
        AquariumSnapshot AddSnapshot(int aquariumId, AquariumSnapshot snapshot, IFormFile snapshotImage);
    }
    public class AquariumService : IAquariumService
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly ILogger<AquariumService> _logger;
        private readonly IDeviceService _deviceService;
        private readonly IConfiguration _config;

        public AquariumService(IConfiguration config,IAquariumDao aquariumDao,IDeviceService deviceService, ILogger<AquariumService> logger)
        {
            _config = config;
            _aquariumDao = aquariumDao;
            _logger = logger;
            _deviceService = deviceService;
        }

        public List<Aquarium> GetAllAquariums()
        {
            return _aquariumDao.GetAquariums();
        }
        public Aquarium AddAquarium(Aquarium aquarium)
        {
            return _aquariumDao.AddAquarium(aquarium);
        }
        public Aquarium UpdateAquarium(Aquarium aquarium)
        {
            return _aquariumDao.UpdateAquarium(aquarium);
        }
        public Aquarium GetAquariumById(int id)
        {
            return _aquariumDao.GetAquariumById(id);
        }
        public void DeleteAquarium(int aquariumId)
        {
            _aquariumDao.DeleteAquarium(aquariumId);
        }

        /* Device Camera configuration */
        public AquariumDevice UpdateDeviceCameraConfiguration(CameraConfiguration config)
        {
            var deviceToUpdate = _aquariumDao.GetAquariumDeviceById(0);
            deviceToUpdate.CameraConfiguration = config;
            return _aquariumDao.UpdateAquariumDevice(deviceToUpdate);
        }


        /* Aquarium Photos */
        public List<AquariumPhoto> GetAquariumPhotos(int aquariumId)
        {
            return _aquariumDao.GetAquariumPhotos(aquariumId).Where(s => s.AquariumId == aquariumId).ToList();
        }

        public void DeleteAquariumPhoto(int photoId)
        {
            _aquariumDao.DeleteAquariumPhoto(photoId);
        }
        public AquariumPhoto GetAquariumPhotoById(int photoId)
        {
            return _aquariumDao.GetAquariumPhotoById(photoId);
        }
        public AquariumPhoto AddAquariumPhoto(AquariumPhoto photo)
        {
            if (!File.Exists(photo.Filepath))
                throw new KeyNotFoundException();
            //Resize image
            using (var img = (Image)new Bitmap(photo.Filepath))
            {
                var folder = Path.GetDirectoryName(photo.Filepath) + "/medium";
                var w = Convert.ToInt16(img.Width * 0.5);
                var h = Convert.ToInt16(img.Height * 0.5);
                var downsized = PhotoResize.ResizeImage(img, w, h);
                downsized.Save(folder);
            }
            using (var img = (Image)new Bitmap(photo.Filepath))
            {
                var folder = Path.GetDirectoryName(photo.Filepath) + "/thumbnail";
                var w = Convert.ToInt16(img.Width * 0.25);
                var h = Convert.ToInt16(img.Height * 0.25);
                var downsized = PhotoResize.ResizeImage(img, w, h);
                downsized.Save(folder);
            }



            return _aquariumDao.AddAquariumPhoto(photo);
        }

        /* Snapshots */
        public List<AquariumSnapshot> GetSnapshots(int aquariumId)
        {
            return _aquariumDao.GetSnapshots().Where(s => s.AquariumId == aquariumId).ToList();
        }
        public AquariumSnapshot GetSnapshotById(int snapshotId)
        {
            return _aquariumDao.GetSnapshotById(snapshotId);
        }
        public void DeleteSnapshot(int snapshotId)
        {
            var snapshot = _aquariumDao.DeleteSnapshot(snapshotId);
        }

        public AquariumSnapshot TakeSnapshot(int aquariumId, bool takePhoto)
        {
            Aquarium aquarium = _aquariumDao.GetAquariumById(aquariumId);
            var deviceId = aquarium.Device.Id;

            AquariumSnapshot snapshot = _deviceService.TakeSnapshot(deviceId);

            if (takePhoto)
            {
                var aquariumPhoto = _deviceService.TakePhoto(deviceId);
                aquariumPhoto.AquariumId = aquarium.Id;
                aquariumPhoto = AddAquariumPhoto(aquariumPhoto);
                snapshot.PhotoId = aquariumPhoto.Id.Value;
            }
            AquariumSnapshot newSnapshot = _aquariumDao.AddSnapshot(snapshot);
            return newSnapshot;
        }
        
        public List<Species> GetAllSpecies()
        {
            return _aquariumDao.GetAllSpecies();
        }
        public Species GetSpeciesById(int speciesId)
        {
            return _aquariumDao.GetSpeciesById(speciesId);
        }
        public Species AddSpecies(Species species)
        {
            return _aquariumDao.AddSpecies(species);
        }
        public Species UpdateSpecies(Species species)
        {
            return _aquariumDao.UpdateSpecies(species);
        }
        public void DeleteSpecies(int speciesId)
        {
            _aquariumDao.DeleteSpecies(speciesId);
        }





        public Fish GetFishById(int fishId)
        {
            return _aquariumDao.GetFishById(fishId);
        }
        public Fish AddFish(Fish fish)
        {
            return _aquariumDao.AddFish(fish);
        }
        public Fish UpdateFish(Fish fish)
        {
            return _aquariumDao.UpdateFish(fish);
        }
        public void DeleteFish(int fishId)
        {
            _aquariumDao.DeleteFish(fishId);
        }


        public Feeding AddFeeding(Feeding feeding)
        {
            //Make sure the fed fish is currently in the tank
            var fish = _aquariumDao.GetFishById(feeding.FishId);
            if (fish.AquariumId != feeding.AquariumId)
                throw new KeyNotFoundException();

            return _aquariumDao.AddFeeding(feeding);
        }
        public Feeding GetFeedingById(int feedingId)
        {
            return _aquariumDao.GetFeedingById(feedingId);
        }
        public List<Feeding> GetFeedingByAquariumId(int aquariumId)
        {
            return _aquariumDao.GetFeedingByAquariumId(aquariumId);
        }
        public Feeding UpdateFeeding(Feeding feeding)
        {
            return _aquariumDao.UpdateFeeding(feeding);
        }
        public void DeleteFeeding(int feedId)
        {
            _aquariumDao.DeleteFeeding(feedId);
        }

        public AquariumDevice AddAquariumDevice(AquariumDevice device)
        {
            var newDevice = _aquariumDao.AddAquariumDevice(device);
            //_deviceService.SetAquarium(newDevice.Id, device.AquariumId);
            return newDevice;
        }
        public AquariumDevice GetAquariumDeviceById(int deviceId)
        {
            return _aquariumDao.GetAquariumDeviceById(deviceId);
        }
        public AquariumDevice GetAquariumDeviceByIpAndKey(string ipAddress,string deviceKey)
        {
            return _aquariumDao.GetAquariumDeviceByIpAndKey(ipAddress, deviceKey);
        }
        public AquariumDevice DeleteAquariumDevice(int deviceId)
        {
            return _aquariumDao.DeleteAquariumDevice(deviceId);
        }
        public AquariumDevice UpdateAquariumDevice(AquariumDevice device)
        {
            var updatedDevice = _aquariumDao.UpdateAquariumDevice(device);
            //_deviceService.SetAquarium(updatedDevice.Id.Value, device.AquariumId.Value);
            return updatedDevice;
        }

        //possibly delete this
        public void SetAquariumDevice(int aquariumId,int deviceId)
        {
            _aquariumDao.SetAquariumDevice(aquariumId, deviceId);
            _deviceService.SetAquarium(deviceId, aquariumId);
        }
        public AquariumDevice ApplyAquariumDeviceHardware(int deviceId, AquariumDevice updatedDevice)
        {
            return _aquariumDao.ApplyAquariumDeviceHardware(deviceId, updatedDevice);
        }
        public AquariumSnapshot AddSnapshot(int aquariumId, AquariumSnapshot snapshot, IFormFile snapshotImage)
        {
            var device = _aquariumDao.GetAquariumById(aquariumId);
            AquariumPhoto photo = null;

            if (snapshotImage != null)
            {
                var downloadPath = String.Format(_config["PhotoFilePath"], aquariumId, DateTimeOffset.Now.ToUnixTimeMilliseconds());
                Directory.CreateDirectory(Path.GetDirectoryName(downloadPath));
                using (Stream output = File.OpenWrite(downloadPath))
                    snapshotImage.CopyTo(output);
                if (!File.Exists(downloadPath))
                    throw new Exception("Could not save photo from request");
                _logger.LogInformation($"Snapshot photo was saved to location: {downloadPath}");
                photo = new AquariumPhoto()
                {
                    Date = new DateTime(),
                    AquariumId = aquariumId,
                    Filepath = downloadPath
                };
                var actualPhoto = AddAquariumPhoto(photo);
                snapshot.PhotoId = actualPhoto.Id;
            }
            snapshot.AquariumId = aquariumId;
            return _aquariumDao.AddSnapshot(snapshot);
        }
    }
}
