using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        List<DeviceSchedule> GetDeviceSchedulesByAccountId(int id);
        AquariumProfile GetProfileById(int profileId);
        BugReport SubmitBugReport(BugReport report);
        void DeleteFish(int fishId);

        Feeding AddFeeding(Feeding feeding);
        AquariumUser GetAccountDetailed(int id, int id1);
        Feeding GetFeedingById(int feedingId);
        List<Feeding> GetFeedingByAquariumId(int aquariumId);
        Feeding UpdateFeeding(Feeding feeding);
        void DeleteFeeding(int feedingId);
        Species GetSpeciesById(int speciesId);

        AquariumDevice AddAquariumDevice(AquariumDevice device);
        AquariumDevice GetAquariumDeviceById(int deviceId);
        AquariumDevice DeleteAquariumDevice(int deviceId);
        void DeleteDeviceSchedule(int scheduleId);
        AquariumDevice UpdateAquariumDevice(AquariumDevice deviceId);
        void SetAquariumDevice(int aquariumId,int deviceId);
        AquariumDevice ApplyAquariumDeviceHardware(int deviceId, AquariumDevice updatedDevice);
        AquariumPhoto GetAquariumPhotoById(int photoId);
        AquariumPhoto AddAquariumPhoto(AquariumPhoto photo);
        List<AquariumPhoto> GetAquariumPhotos(int aquariumId,PaginationSliver pagination);
        AquariumDevice GetAquariumDeviceByIpAndKey(string ipAddress,string deviceKey);
        AquariumDevice UpdateDeviceCameraConfiguration(CameraConfiguration config);
        AquariumSnapshot AddSnapshot(int aquariumId, AquariumSnapshot snapshot, IFormFile snapshotImage);
        DeviceSchedule UpdateDeviceSchedule(DeviceSchedule deviceSchedule);
        FishPhoto AddFishPhoto(int fishId,IFormFile photo);
        FishPhoto GetFishPhotoById(int photoId);
        AccountRelationship GetAccountRelationship(int aquariumId, int targetId);
        AccountRelationship UpsertFollowUser(int aquariumId, int targetId);
        List<SearchResult> PerformSearch(SearchOptions options);
        List<Aquarium> GetAquariumsByAccountId(int userId);
        List<AquariumSnapshot> GetAquariumTemperatureHistogram(int id);
        List<AquariumSnapshot> GetAquariumSnapshots(int aquariumId,int offset, int max);
        DeviceSchedule AddDeviceSchedule(DeviceSchedule deviceSchedule);
        List<DeviceScheduleAssignment> DeployDeviceSchedule(int deviceId, int scheduleId);
        List<DeviceScheduleAssignment> RemoveDeviceSchedule(int deviceId, int scheduleId);
        void DeleteAllSnapshots(int aquariumId);
        void DeleteSnapshots(List<int> snapshotIds);
        void DeleteAquariumPhotos(List<int> aquariumPhotoIds);
        List<AquariumPhoto> GetAquariumPhotosByAccount(int accountId);
        List<AquariumSnapshot> GetAquariumSnapshotPhotos(int aquariumId, PaginationSliver pagination);
        List<FishPhoto> GetAquariumFishPhotos(int aquariumId, PaginationSliver pagination);
        AquariumPhoto AddAquariumPhoto(int aquariumId, IFormFile photo);
        WaterChange AddWaterChange(WaterChange waterChange);
        ICollection<WaterChange> GetWaterChangesByAquarium(int aquariumId);
        WaterChange UpdateWaterChange(WaterChange waterChange);
        void DeleteWaterChanges(List<int> waterChangeIds);
        ICollection<WaterDosing> GetWaterDosingsByAquarium(int aquariumId);
        WaterDosing AddWaterDosing(WaterDosing waterDosing);
        WaterDosing UpdateWaterDosing(WaterDosing waterDosing);
        void DeleteWaterDosings(List<int> waterDosingIds);
    }
    public class AquariumService : IAquariumService
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly ILogger<AquariumService> _logger;
        private readonly IDeviceService _deviceService;
        private readonly IPhotoManager _photoManager;
        private readonly IConfiguration _config;
        private readonly IActivityService _activityService;
        private readonly IAccountService _accountService;

        public AquariumService(IConfiguration config, ILogger<AquariumService> logger, IAquariumDao aquariumDao, IAccountService accountService,
                               IActivityService activityService,IDeviceService deviceService,IPhotoManager photoManager)
        {
            _config = config;
            _activityService = activityService;
            _accountService = accountService;
            _aquariumDao = aquariumDao;
            _logger = logger;
            _deviceService = deviceService;
            _photoManager = photoManager;
        }

        public List<Aquarium> GetAllAquariums()
        {
            return _aquariumDao.GetAquariums();
        }
        public Aquarium AddAquarium(Aquarium aquarium)
        {
            aquarium.Name = aquarium.Name.Trim();
            aquarium.StartDate = aquarium.StartDate.ToUniversalTime();
            if (aquarium.Name == null) throw new Exception("Invalid aquarium name");
            if (aquarium.Gallons <= 0) throw new Exception("Invalid aquarium size");

            var newAquarium = _aquariumDao.AddAquarium(aquarium);

            var activity = new CreateAquariumActivity()
            {
                AccountId = newAquarium.OwnerId,
                AquariumId = newAquarium.Id
            };
            _activityService.RegisterActivity(activity);
            return newAquarium;
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
            var aq = _aquariumDao.GetAquariumById(aquariumId);
            _aquariumDao.DeleteAquarium(aquariumId);

            var activity = new DeleteAquariumActivity()
            {
                AccountId = aq.OwnerId,
                AquariumId = aquariumId
            };
            _activityService.RegisterActivity(activity);
        }


        /* Device Camera configuration */
        public AquariumDevice UpdateDeviceCameraConfiguration(CameraConfiguration config)
        {
            var deviceToUpdate = _aquariumDao.GetAquariumDeviceById(0);
            deviceToUpdate.CameraConfiguration = config;
            return _aquariumDao.UpdateAquariumDevice(deviceToUpdate);
        }


        /* Snapshots */
        public List<AquariumSnapshot> GetSnapshots(int aquariumId)
        {
            var snapshots = _aquariumDao.GetSnapshotsByAquarium(aquariumId);
            return snapshots;
        }
        public AquariumSnapshot GetSnapshotById(int snapshotId)
        {
            return _aquariumDao.GetSnapshotById(snapshotId);
        }
        public void DeleteSnapshot(int snapshotId)
        {
            var snapshot = GetSnapshotById(snapshotId);
            if (snapshot.PhotoId.HasValue)
                _photoManager.DeletePhoto(snapshot.PhotoId.Value);
            _aquariumDao.DeleteSnapshot(snapshot.Id);
        }

        public AquariumSnapshot TakeSnapshot(int aquariumId, bool takePhoto)
        {
            Aquarium aquarium = _aquariumDao.GetAquariumById(aquariumId);
            var deviceId = aquarium.Device.Id;

            AquariumSnapshot snapshot = _deviceService.TakeSnapshot(deviceId); //todo tell device to take with image

            if (takePhoto)
            {
                var photoData = _deviceService.TakePhoto(deviceId);
                var photo = _photoManager.StorePhoto(photoData).Result;
                snapshot.PhotoId = photo.Id;
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
            species.Name = species.Name.Trim();
            if (species.Name == null)
                throw new Exception("Species must have a name");
            var exists = _aquariumDao.GetAllSpecies().Where(s => s.Name == species.Name).Any(); //todo move this into new method maybe
            if(exists)
                throw new Exception("Species with this name already exists");

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
            fish.Name = fish.Name.Trim();

            fish.Description = fish.Description?.Trim();

            if (string.IsNullOrEmpty(fish.Name))
                throw new InvalidDataException();

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
            if (snapshotImage != null)
            {
                var stream = snapshotImage.OpenReadStream();
                using (var ms = new MemoryStream())
                {
                    snapshotImage.CopyTo(ms);
                    var buffer = ms.ToArray();
                    var photo = _photoManager.StorePhoto(buffer).Result;
                    snapshot.PhotoId = photo.Id;
                }
            }
            snapshot.AquariumId = aquariumId;
            return _aquariumDao.AddSnapshot(snapshot);
        }

        /* Photos */
        /* Aquarium Photos */
        public List<AquariumPhoto> GetAquariumPhotos(int aquariumId,PaginationSliver pagination)
        {
            var photos = _aquariumDao.GetAquariumPhotos(aquariumId);
            if (pagination.Descending)
                photos = photos.OrderByDescending(p => p.Photo.Date);
            var sliver = photos.Skip(pagination.Start).Take(pagination.Count);
            return sliver.ToList();
        }
        public List<AquariumSnapshot> GetAquariumSnapshotPhotos(int aquariumId, PaginationSliver pagination)
        {
            var photos = _aquariumDao.GetSnapshotsByAquarium(aquariumId)
                .Where(s => s.Photo != null);
            if (pagination.Descending)
                photos = photos.OrderByDescending(p => p.Photo.Date);
            var sliver = photos.Skip(pagination.Start).Take(pagination.Count);
            return sliver.ToList();
        }
        public List<FishPhoto> GetAquariumFishPhotos(int aquariumId, PaginationSliver pagination)
        {
            var photos = _aquariumDao.GetAquariumFishPhotos(aquariumId);
            if (pagination.Descending)
                photos = photos.OrderByDescending(p => p.Photo.Date);
            var sliver = photos.Skip(pagination.Start).Take(pagination.Count);
            return sliver.ToList();
        }
        public List<AquariumPhoto> GetAquariumPhotosByAccount(int accountId)
        {
            return _aquariumDao.GetAquariumPhotosByAccount(accountId).ToList();
        }

        public AquariumPhoto GetAquariumPhotoById(int photoId)
        {
            AquariumPhoto aquariumPhoto = _aquariumDao.GetAquariumPhotoById(photoId);
            return aquariumPhoto;
        }
        public AquariumPhoto AddAquariumPhoto(AquariumPhoto photo)
        {
            return _aquariumDao.AddAquariumPhoto(photo);
        }
        public AquariumPhoto AddAquariumPhoto(int aquariumId, IFormFile photo)
        {
            var stream = photo.OpenReadStream();
            PhotoContent photoContent;

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var buffer = ms.ToArray();
                photoContent = _photoManager.StorePhoto(buffer).Result;
            }
            return _aquariumDao.AddAquariumPhoto(new AquariumPhoto
            {
                AquariumId = aquariumId,
                PhotoId = photoContent.Id
            });
        }
        public FishPhoto AddFishPhoto(int fishId, IFormFile photo)
        {
            var stream = photo.OpenReadStream();
            PhotoContent photoContent;

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var buffer = ms.ToArray();
                photoContent = _photoManager.StorePhoto(buffer).Result;
            }
            return _aquariumDao.AddFishPhoto(new FishPhoto
            {
                FishId = fishId,
                PhotoId = photoContent.Id
            });
        }
        public FishPhoto GetFishPhotoById(int photoId)
        {
            FishPhoto fishPhoto = _aquariumDao.GetFishPhotoById(photoId);
            return fishPhoto;
        }
        public BugReport SubmitBugReport(BugReport report)
        {
            report.Date = DateTime.Now.ToUniversalTime();
            report.Status = "Submitted";
            return _aquariumDao.AddBugReport(report);
        }

        public AquariumUser GetAccountDetailed(int senderId,int targetId)
        {
            //todo determine relationship, what details can they view
            return _aquariumDao.GetAccountById(targetId);
        }

        public AquariumProfile GetProfileById(int profileId)
        {
            var profile = _aquariumDao.GetProfileById(profileId);
            profile.Fish = profile.Aquariums.SelectMany(a => a.Fish).ToList();
            profile.Activity = _activityService.GetRecentActivity(profileId);
            profile.Relationship = _aquariumDao.GetAccountRelationship(_accountService.GetCurrentUserId(), profileId);
            return profile;
        }

        public AccountRelationship GetAccountRelationship(int aquariumId,int  targetId)
        {
            return _aquariumDao.GetAccountRelationship(aquariumId,targetId);
        }
        public AccountRelationship UpsertFollowUser(int aquariumId, int targetId)
        {
            return _aquariumDao.UpsertFollowUser(aquariumId, targetId);

        }

        public List<SearchResult> PerformSearch(SearchOptions options)
        {
            return _aquariumDao.PerformSearch(options);
        }
        public List<Aquarium> GetAquariumsByAccountId(int userId)
        {
            return _aquariumDao.GetAquariumsByAccountId(userId);

        }
        public List<AquariumSnapshot> GetAquariumTemperatureHistogram(int id)
        {
            return _aquariumDao.GetAquariumTemperatureHistogram(id);
        }
        public List<AquariumSnapshot> GetAquariumSnapshots(int aquariumId,int offset, int max)
        {
            return _aquariumDao.GetAquariumSnapshots(aquariumId,offset, max);
        }




        /* Device Schedule */
        public List<DeviceSchedule> GetDeviceSchedulesByAccountId(int id)
        {
            return _aquariumDao.GetDeviceSchedulesByAccount(id);
        }
        public void DeleteDeviceSchedule(int scheduleId)
        {
            var affectedDevices = _aquariumDao.GetDevicesInUseBySchedule(scheduleId);
            _aquariumDao.DeleteDeviceSchedule(scheduleId);
            affectedDevices.ForEach(device =>
            {
                try
                {

                    _deviceService.ApplyScheduleAssignment(device.Id, _aquariumDao.GetAssignedDeviceSchedules(device.Id).Select(sa => sa.Schedule).ToList());
                }
                catch (Exception e)
                {
                    //todo could not update schedule assignment (pi is offline maybe)
                }
            });

        }
        public DeviceSchedule AddDeviceSchedule(DeviceSchedule deviceSchedule)
        {
            return _aquariumDao.AddDeviceSchedule(deviceSchedule);
        }
        public List<DeviceScheduleAssignment> DeployDeviceSchedule(int deviceId, int scheduleId)
        {
            _aquariumDao.AssignDeviceSchedule(scheduleId, deviceId);
            var assignments = _aquariumDao.GetAssignedDeviceSchedules(deviceId);
            try
            {

                _deviceService.ApplyScheduleAssignment(deviceId, assignments.Select(sa => sa.Schedule).ToList());
            }
            catch (Exception e)
            {
                _logger.LogError("Could not apply schedule assignment");
            }
            return assignments;

        }
        public List<DeviceScheduleAssignment> RemoveDeviceSchedule(int deviceId, int scheduleId)
        {
            _aquariumDao.UnassignDeviceSchedule(scheduleId, deviceId);
            var assignments = _aquariumDao.GetAssignedDeviceSchedules(deviceId);

            try
            {

             _deviceService.ApplyScheduleAssignment(deviceId, assignments.Select(sa => sa.Schedule).ToList());
            }
            catch(Exception e)
            {
                _logger.LogError("Could not apply schedule assignment");
            }


            return assignments;
        }
        public DeviceSchedule UpdateDeviceSchedule(DeviceSchedule deviceSchedule)
        {
            var updatedSchedule = _aquariumDao.UpdateDeviceSchedule(deviceSchedule);
            UpdateDeviceSchedulesByScheduleId(deviceSchedule.Id);
            return updatedSchedule;
        }
        public void UpdateDeviceSchedulesByScheduleId(int scheduleId)
        {
            var affectedDevices = _aquariumDao.GetDevicesInUseBySchedule(scheduleId);
            affectedDevices.ForEach(device =>
            {
                try
                {

                _deviceService.ApplyScheduleAssignment(device.Id, _aquariumDao.GetAssignedDeviceSchedules(device.Id).Select(sa => sa.Schedule).ToList());
                }
                catch(Exception e)
                {
                    //todo could not update schedule assignment (pi is offline maybe)
                }
            });
        }

        public void DeleteAllSnapshots(int aquariumId)
        {
            _aquariumDao.DeleteAllSnapshots(aquariumId);
        }

        public void DeleteSnapshots(List<int> snapshotIds)
        {
            _aquariumDao.DeleteSnapshots(snapshotIds);
        }
        public void DeleteAquariumPhotos(List<int> aquariumPhotoIds)
        {
            _aquariumDao.DeleteAquariumPhotos(aquariumPhotoIds);
        }

        public ICollection<WaterChange> GetWaterChangesByAquarium(int aquariumId)
        {
            return _aquariumDao.GetWaterChangesByAquarium(aquariumId);
        }
        public WaterChange AddWaterChange(WaterChange waterChange)
        {
            return _aquariumDao.AddWaterChange(waterChange);
        }
        public WaterChange UpdateWaterChange(WaterChange waterChange)
        {
            return _aquariumDao.UpdateWaterChange(waterChange);
        }
        public void DeleteWaterChanges(List<int> waterChangeIds)
        {
            _aquariumDao.DeleteWaterChanges(waterChangeIds);
        }
        public ICollection<WaterDosing> GetWaterDosingsByAquarium(int aquariumId)
        {
            return _aquariumDao.GetWaterDosingsByAquarium(aquariumId);
        }
        public WaterDosing AddWaterDosing(WaterDosing waterDosing)
        {
            return _aquariumDao.AddWaterDosing(waterDosing);
        }
        public WaterDosing UpdateWaterDosing(WaterDosing waterDosing)
        {
            return _aquariumDao.UpdateWaterDosing(waterDosing);
        }
        public void DeleteWaterDosings(List<int> waterDosingIds)
        {
             _aquariumDao.DeleteWaterDosings(waterDosingIds);
        }
    }
}
