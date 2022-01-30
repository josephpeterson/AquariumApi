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
    public partial interface IAquariumService
    {
        /* Aquarium */
        Aquarium AddAquarium(Aquarium aquarium);
        List<Aquarium> GetAllAquariums();
        Aquarium GetAquariumById(int id);
        Aquarium UpdateAquarium(Aquarium aquarium);
        void DeleteAquarium(int aquariumId);

        AquariumSnapshot TakeSnapshot(int aquariumId,bool takePhoto);
        List<AquariumSnapshot> GetSnapshots(int aquariumId);
        AquariumSnapshot GetSnapshotById(int snapshotId);
        void DeleteSnapshot(int removeSnapshotId);
        AquariumSnapshot AddSnapshot(int aquariumId, AquariumSnapshot snapshot, IFormFile snapshotImage);





        AquariumProfile GetProfileById(int profileId);
        BugReport SubmitBugReport(BugReport report);

        AquariumUser GetAccountDetailed(int id, int id1);



        FishPhoto AddFishPhoto(int fishId,IFormFile photo);
        FishPhoto GetFishPhotoById(int photoId);
        AccountRelationship GetAccountRelationship(int aquariumId, int targetId);
        AccountRelationship UpsertFollowUser(int aquariumId, int targetId);
        List<SearchResult> PerformSearch(SearchOptions options);
        List<Aquarium> GetAquariumsByAccountId(int userId);
        List<AquariumSnapshot> GetAquariumTemperatureHistogram(int id);
        List<AquariumSnapshot> GetAquariumSnapshots(int aquariumId,int offset, int max);
        List<AquariumSnapshot> GetSnapshotsByIds(List<int> snapshotIds);

        void DeleteAllSnapshots(int aquariumId);
        void DeleteSnapshots(List<int> snapshotIds);



        void DeleteAquariumPhotos(List<int> aquariumPhotoIds);
        List<AquariumPhoto> GetAquariumPhotosByAccount(int accountId);
        AquariumPhoto GetAquariumPhotoById(int photoId);
        AquariumPhoto AddAquariumPhoto(AquariumPhoto photo);
        List<AquariumPhoto> GetAquariumPhotos(int aquariumId, PaginationSliver pagination);


        List<AquariumSnapshot> GetAquariumSnapshotPhotos(int aquariumId, PaginationSliver pagination);
        List<FishPhoto> GetAquariumFishPhotos(int aquariumId, PaginationSliver pagination);
        AquariumPhoto AddAquariumPhoto(int aquariumId, IFormFile photo);

        /* Form Components */
        List<KeyValuePair<string,int>> GetSelectOptionsBySelectType(string selectType);
        
    }
    public partial class AquariumService : IAquariumService
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly ILogger<AquariumService> _logger;
        private readonly IDeviceClient _deviceClient;
        private readonly IPhotoManager _photoManager;

        private readonly INotificationService _notificationService;



        private readonly IConfiguration _config;
        private readonly IActivityService _activityService;
        private readonly IAccountService _accountService;

        public AquariumService(IConfiguration config, ILogger<AquariumService> logger, IAquariumDao aquariumDao, IAccountService accountService,
                               IActivityService activityService,IDeviceClient deviceClient,IPhotoManager photoManager,INotificationService notificationService)
        {
            _config = config;
            _activityService = activityService;
            _accountService = accountService;
            _aquariumDao = aquariumDao;
            _logger = logger;
            _deviceClient = deviceClient;
            _photoManager = photoManager;
            _notificationService = notificationService;
        }

        public List<Aquarium> GetAllAquariums()
        {
            return _aquariumDao.GetAquariums();
        }
        public Aquarium AddAquarium(Aquarium aquarium)
        {
            aquarium.Name = aquarium.Name.Trim();
            aquarium.StartDate = aquarium.StartDate.ToUniversalTime();

            var type = aquarium.Substrate.Type.ToLower();
            if (type == "n/a" || type == "none")
                aquarium.Substrate.Type = null;


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
        public List<AquariumSnapshot> GetSnapshotsByIds(List<int> snapshotIds)
        {
            return _aquariumDao.GetSnapshotsByIds(snapshotIds).ToList();
        }
        public void DeleteSnapshot(int snapshotId)
        {
            var snapshot = GetSnapshotById(snapshotId);
            if (snapshot.PhotoId.HasValue)
                _photoManager.DeletePhoto(snapshot.PhotoId.Value);
            _aquariumDao.DeleteSnapshot(snapshot.Id.Value);
        }

        public AquariumSnapshot TakeSnapshot(int aquariumId, bool takePhoto)
        {
            Aquarium aquarium = _aquariumDao.GetAquariumById(aquariumId);
            var deviceId = aquarium.Device.Id;

            AquariumSnapshot snapshot = _deviceClient.TakeSnapshot(deviceId); //todo tell device to take with image

            if (takePhoto)
            {
                var photoData = _deviceClient.TakePhoto(deviceId);
                var photo = _photoManager.StorePhoto(photoData).Result;
                snapshot.PhotoId = photo.Id;
            }
            AquariumSnapshot newSnapshot = _aquariumDao.AddSnapshot(snapshot);
            return newSnapshot;
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
        //todo maybe dynamically generate this
        public List<KeyValuePair<string, int>> GetSelectOptionsBySelectType(string selectType)
        {
            List<KeyValuePair<string, int>> options;
            switch (selectType)
            {
                case "SpeciesCategories":
                    options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Freshwater",0),
                        new KeyValuePair<string, int>("Marine",1),
                        new KeyValuePair<string, int>("Reef Compatible",2),
                    };
                    break;
                case "SpeciesTemperament":
                    options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Peaceful",0),
                        new KeyValuePair<string, int>("Semi-Aggressive",1),
                        new KeyValuePair<string, int>("Aggressive",2),
                    };
                    break;
                case "SpeciesCareLevel":
                    options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Easy",0),
                        new KeyValuePair<string, int>("Intermediate",1),
                        new KeyValuePair<string, int>("Expert",2),
                    };
                    break;
                case "DeviceSensorTypes":
                    options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Float Switch",(int)SensorTypes.FloatSwitch),
                        new KeyValuePair<string, int>("Solenoid Relay",(int)SensorTypes.Solenoid),
                        new KeyValuePair<string, int>("Water Change Pump Relay",(int)SensorTypes.WaterChangePumpRelay),
                        new KeyValuePair<string, int>("ATO Pump Relay",(int)SensorTypes.ATOPumpRelay),
                        new KeyValuePair<string, int>("Other",(int)SensorTypes.Other),
                    };
                    break;
                case "DeviceSensorValues":
                    options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Low",(int)GpioPinValue.Low),
                        new KeyValuePair<string, int>("High",(int)GpioPinValue.High),
                    };
                    break;
                case "DeviceTaskTypes":
                    options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Take Snapshot",(int)ScheduleTaskTypes.Snapshot),
                        new KeyValuePair<string, int>("Start ATO",(int)ScheduleTaskTypes.StartATO),
                        new KeyValuePair<string, int>("Water Change Drain",(int)ScheduleTaskTypes.WaterChangeDrain),
                        new KeyValuePair<string, int>("Water Change Replentish",(int)ScheduleTaskTypes.WaterChangeReplentish),
                        new KeyValuePair<string, int>("Unknown",(int)ScheduleTaskTypes.Unknown),
                    };
                    break;
                case "TriggerTypes":
                    options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Start at Time",(int)TriggerTypes.Time),
                        new KeyValuePair<string, int>("Sensor Condition",(int)TriggerTypes.SensorDependent),
                        new KeyValuePair<string, int>("Task Condition",(int)TriggerTypes.TaskDependent),
                        new KeyValuePair<string, int>("Task Assignment",(int)TriggerTypes.TaskAssignmentCompleted),
                    };
                    break;
                default:
                    options = new List<KeyValuePair<string, int>>();
                    break;
            }

            return options;
        }

    }
}