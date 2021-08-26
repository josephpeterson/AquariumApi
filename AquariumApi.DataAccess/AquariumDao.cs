using AquariumApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using System;
using System.Threading.Tasks;

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
        List<AquariumSnapshot> GetSnapshotsByAquarium(int aquariumId);
        AquariumSnapshot GetSnapshotById(int snapshotId);
        AquariumSnapshot DeleteSnapshot(int snapshotId);

        Species AddSpecies(Species species);
        List<Species> GetAllSpecies();
        Species UpdateSpecies(Species species);
        void DeleteSpecies(int speciesId);
        void RegisterActivity(Activity newActivity);
        Species GetSpeciesById(int speciesId);

        Fish AddFish(Fish fish);
        Fish GetFishById(int fishId);
        Fish UpdateFish(Fish fish);
        void DeleteFish(int fishId);
        FishPhoto AddFishPhoto(FishPhoto photo);


        Feeding AddFeeding(Feeding feeding);
        Feeding GetFeedingById(int feedingId);
        List<Feeding> GetFeedingByAquariumId(int aquariumId);
        AquariumUser AddAccount(SignupRequest signupRequest);
        Feeding UpdateFeeding(Feeding feeding);
        void DeleteDispatchedNotification(int notificationId);
        FishDisease AddDiseaseDiagnosis(FishDisease diseaseDiagnois);
        void DeleteFeeding(int feedingId);

        void SetAquariumDevice(int aquariumId,int deviceId);
        AquariumDevice AddAquariumDevice(AquariumDevice device);
        AquariumDevice UpdateAquariumDevice(AquariumDevice device);
        void DeleteNotification(int notificationId);
        void DismissDispatchedNotification(int notificationId);
        void DismissDispatchedNotifications(List<int> notificationIds);
        void DeleteDispatchedNotifications(List<int> notificationIds);
        AquariumDevice DeleteAquariumDevice(int deviceId);
        void DismissNotifications(List<int> notificationIds);
        AquariumDevice GetAquariumDeviceById(int deviceId);
        Task EmitNotification(DispatchedNotification notif);
        AquariumDevice ApplyAquariumDeviceHardware(int deviceId, AquariumDevice updatedDevice);

        IEnumerable<AquariumPhoto> GetAquariumPhotos(int aquariumId);
        Task EmitNotification(DispatchedNotification notif, List<int> aquariumAccountIds);
        void DeleteAquariumPhoto(int photoId);
        ICollection<DispatchedNotification> GetAllDispatchedNotifications();
        FishBreeding AddBreeding(FishBreeding breeding);
        void DeleteFishPhoto(int photoId);
        AquariumPhoto AddAquariumPhoto(AquariumPhoto photo);
        AquariumDevice GetAquariumDeviceByIpAndKey(string ipAddress,string deviceKey);

        List<AquariumOverviewResponse> GetAquariumOverviews();
        ICollection<Notification> GetNotificationsByAccountId(int id);

        AquariumUser GetAccountByLogin(int userId, string password);
        ICollection<Notification> GetNotificationsById(List<int> notificationIds);
        Fish MarkDeseased(FishDeath death);
        AquariumUser GetAccountById(int userId);
        List<AquariumUser> GetAllAccounts();
        Fish TransferFish(int fishId, int aquariumId);
        AquariumUser GetUserByUsername(string username);
        AquariumUser GetUserByEmail(string email);
        List<Fish> GetAllFishByAccount(int accountId);
        FishPhoto GetFishPhotoById(int photoId);
        BugReport AddBugReport(BugReport report);
        List<BugReport> GetAllBugs();
        AquariumProfile GetProfileById(int targetId);
        AquariumProfile UpdateProfile(AquariumProfile profile);

        List<Activity> GetRecentAccountActivity(int accountId);
        Activity GetAccountActivity(int activityId);
        AquariumUser UpdateUser(AquariumUser user);
        AccountRelationship GetAccountRelationship(int aquariumId, int targetId);
        AccountRelationship UpsertFollowUser(int aquariumId, int targetId);
        void DeletePostCategory(int categoryId);
        List<SearchResult> PerformSearch(SearchOptions options);
        List<PostCategory> GetPostCategories();
        PostBoard GetBoardById(int boardId);
        PostThread GetThreadById(int threadId);
        Post CreatePost(Post post);
        Post GetPostById(int postId);
        PostCategory CreatePostCategory(PostCategory category);
        PostBoard CreatePostBoard(PostBoard board);
        AquariumUser GetUserByUsernameOrEmail(string email);
        PostThread CreatePostThread(PostThread thread);
        void DeletePostBoard(int boardId);
        void DeletePostThread(int threadId);
        void DeletePost(int postId);
        void UpdatePasswordForUser(int uId, string newPassword);
        List<AquariumSnapshot> GetAquariumTemperatureHistogram(int aquariumId);
        List<Aquarium> GetAquariumsByAccountId(int userId);
        PhotoContent CreatePhotoReference();
        PhotoContent UpdatePhotoReference(PhotoContent content);
        void DeletePhoto(int photoId);
        PhotoContent GetPhoto(int photoId);
        List<AquariumSnapshot> GetAquariumSnapshots(int aquariumId,int offset, int max);

        /* Device Schedules */
        DeviceSchedule UpdateDeviceSchedule(DeviceSchedule deviceSchedule);
        void DeleteDeviceSchedule(int scheduleId);
        List<DeviceSchedule> GetDeviceSchedulesByAccount(int accountId);
        DeviceScheduleAssignment AssignDeviceSchedule(int scheduleId, int deviceId);
        void UnassignDeviceSchedule(int scheduleId, int deviceId);
        DeviceSchedule AddDeviceSchedule(DeviceSchedule deviceSchedule);
        List<DeviceScheduleAssignment> GetAssignedDeviceSchedules(int deviceId);
        List<AquariumDevice> GetDevicesInUseBySchedule(int scheduleId);
        void DeleteAllSnapshots(int aquariumId);
        void DeleteSnapshots(List<int> snapshotIds);
        AquariumPhoto GetAquariumPhotoById(int photoId);
        List<AquariumPhoto> GetAquariumPhotosByAccount(int accountId);
        void DeleteAquariumPhotos(List<int> photoIds);
        IEnumerable<FishPhoto> GetAquariumFishPhotos(int aquariumId);
        ICollection<WaterChange> GetWaterChangesByAquarium(int aquariumId);
        WaterChange AddWaterChange(WaterChange waterChange);
        WaterChange UpdateWaterChange(WaterChange waterChange);
        void DeleteWaterChanges(List<int> waterChangeIds);
        ICollection<WaterDosing> GetWaterDosingsByAquarium(int aquariumId);
        WaterDosing AddWaterDosing(WaterDosing waterDosing);
        WaterDosing UpdateWaterDosing(WaterDosing waterDosing);
        void DeleteWaterDosings(List<int> waterDosingIds);
        List<DeviceScheduleAssignment> GetScheduleAssignmentBySchedule(int scheduleId);
        List<PhotoContent> GetPhotoContentByIds(int[] photoIds);
        ICollection<AquariumSnapshot> GetSnapshotsByIds(List<int> snapshotIds);
        ICollection<DeviceSensor> GetDeviceSensors(int deviceId);
        DeviceSensor AddDeviceSensor(DeviceSensor deviceSensor);
        DeviceSensor UpdateDeviceSensor(DeviceSensor deviceSensor);
        void DeleteDeviceSensors(List<int> deviceSensorIds);
        ATOStatus UpdateATOStatus(ATOStatus atoStatus);
        ATOStatus AddATOStatus(ATOStatus atoStatus);
        List<ATOStatus> GetATOHistory(int deviceId,PaginationSliver sliver);
        ICollection<AquariumSnapshot> GetWaterParametersByAquarium(int aquariumId, PaginationSliver pagination);
        List<ATOStatus> GetWaterATOByAquarium(int aquariumId, PaginationSliver pagination);
    }

    public class AquariumDao : IAquariumDao
    {
        private readonly DbAquariumContext _dbAquariumContext;
        private readonly ILogger<AquariumDao> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AquariumDao(IMapper mapper, IConfiguration config, DbAquariumContext dbAquariumContext, ILogger<AquariumDao> logger)
        {
            _mapper = mapper;
            _config = config;
            _dbAquariumContext = dbAquariumContext;
            _logger = logger;
        }

        /* Aquarium Accounts */
        public AquariumUser GetAccountByLogin(int userId, string password)
        {
            var acc = _dbAquariumContext.TblSignupRequests.AsNoTracking()
                 .Where(p => p.Id == userId && p.Password == password)
                 .First();
            return GetAccountById(acc.Id);
        }
        public AquariumUser GetAccountById(int userId)
        {
            return _dbAquariumContext.TblAccounts.AsNoTracking()
                .Where(p => p.Id == userId)
                .SingleOrDefault();
        }
        public List<AquariumUser> GetAllAccounts()
        {
            return _dbAquariumContext.TblAccounts.AsNoTracking().ToList();
        }
        public AquariumUser GetUserByUsername(string username)
        {
            return _dbAquariumContext.TblAccounts.AsNoTracking()
                .Where(p => p.Username == username)
                .SingleOrDefault();
        }
        public AquariumUser GetUserByEmail(string email)
        {
            return _dbAquariumContext.TblAccounts.AsNoTracking()
                .Where(p => p.Email == email)
                .SingleOrDefault();
        }
        public AquariumUser UpdateUser(AquariumUser user)
        {
            var userToUpdate = _dbAquariumContext.TblAccounts.SingleOrDefault(u => user.Id == u.Id);
            if (userToUpdate == null)
                throw new KeyNotFoundException();

            userToUpdate = _mapper.Map(user, userToUpdate);
            _dbAquariumContext.SaveChanges();
            return GetUserByEmail(userToUpdate.Email);
        }
        public AquariumUser GetUserByUsernameOrEmail(string email)
        {
            return _dbAquariumContext.TblAccounts.AsNoTracking()
                .Where(p => p.Username == email || p.Email == email)
                .SingleOrDefault();
        }
        public AquariumUser AddAccount(SignupRequest signupRequest)
        {
            _dbAquariumContext.TblSignupRequests.Add(signupRequest);
            _dbAquariumContext.SaveChanges();
            return GetAccountById(signupRequest.Id);
        }
        public void UpdatePasswordForUser(int uId, string newPassword)
        {
            var acc = _dbAquariumContext.TblSignupRequests.Where(s => s.Id == uId).First();
            acc.Password = newPassword;
            _dbAquariumContext.SaveChanges();
        }
        public AquariumProfile GetProfileById(int targetId)
        {
            var results = _dbAquariumContext.TblAquariumProfiles.AsNoTracking()
                .Where(r => r.Id == targetId)
                .Include(p => p.Thumbnail)
                .Include(e => e.Aquariums).ThenInclude(e => e.Fish)
                .Include(e => e.Account);
            var profile = results.First();
            return profile;
        }
        public AquariumProfile UpdateProfile(AquariumProfile profile)
        {
            _dbAquariumContext.TblAquariumProfiles.Update(profile);
            _dbAquariumContext.SaveChanges();
            return profile;
        }
        public AccountRelationship GetAccountRelationship(int aquariumId, int targetId)
        {
            if (aquariumId == targetId)
                return null;

            var relationship = _dbAquariumContext.TblAccountRelationship.Where(rel => rel.AccountId == aquariumId && rel.TargetId == targetId).FirstOrDefault();
            if (relationship == null)
            {
                relationship = new AccountRelationship()
                {
                    AccountId = aquariumId,
                    TargetId = targetId,
                    Relationship = 0
                };
            }
            return relationship;
        }
        public AccountRelationship UpsertFollowUser(int aquariumId, int targetId)
        {
            if (aquariumId == targetId)
                return null;
            var rel = _dbAquariumContext.TblAccountRelationship.Where(r => r.AccountId == aquariumId && r.TargetId == targetId).FirstOrDefault();
            if (rel == null)
                _dbAquariumContext.TblAccountRelationship.Add(rel = new AccountRelationship()
                {
                    AccountId = aquariumId,
                    TargetId = targetId,
                    Relationship = RelationshipTypes.Following
                });
            else if (rel.Relationship == RelationshipTypes.Following)
                rel.Relationship = RelationshipTypes.None;
            else
                rel.Relationship = RelationshipTypes.Following;
            _dbAquariumContext.SaveChanges();
            return rel;
        }

        /* Account Activity */
        public void RegisterActivity(Activity newActivity)
        {
            _dbAquariumContext.TblAccountActivity.Add(newActivity);
            _dbAquariumContext.SaveChanges();
        }
        public List<Activity> GetRecentAccountActivity(int accountId)
        {
            return _dbAquariumContext.TblAccountActivity.Where(activity => activity.AccountId == accountId
            && activity.ActivityType != ActivityTypes.LoginAccountActivity)
            .OrderByDescending(act => act.Timestamp)
            .Take(100)
            .ToList();
        }
        public Activity GetAccountActivity(int activityId)
        {
            return _dbAquariumContext.TblAccountActivity.Where(activity => activity.Id == activityId).First();
        }

        /* Aquarium */
        public List<Aquarium> GetAquariumsByAccountId(int userId)
        {
            return _dbAquariumContext.TblAquarium
                .Include(aq => aq.Device)
                .Where(aq => aq.OwnerId == userId)
                .ToList();
        }
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
                .Include(aq => aq.Equipment)
                .Include(aq => aq.Substrate)
                .Include(aq => aq.Plan)
                .Include(aq => aq.Fish).ThenInclude(d => d.Species)
                .Include(aq => aq.Fish).ThenInclude(d => d.Thumbnail)
                .Include(aq => aq.Feedings)
                .Include(aq => aq.Device).ThenInclude(d => d.CameraConfiguration)
                .Include(aq => aq.Device).ThenInclude(d => d.ScheduleAssignments)
                    .ThenInclude(sa => sa.Schedule).ThenInclude(s => s.Tasks)
                .FirstOrDefault();
            if (aquarium == null) return aquarium;

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
            var aquarium = _dbAquariumContext.TblAquarium.Where(aq => aq.Id == aquariumId)
                .Include(aq => aq.Equipment)
                .Include(aq => aq.Substrate)
                .Include(aq => aq.Plan)
                .Include(aq => aq.Device)
                .Include(aq => aq.Fish).First();

            _dbAquariumContext.TblSnapshot.RemoveRange(_dbAquariumContext.TblSnapshot.Where(aq => aq.AquariumId == aquariumId));
            _dbAquariumContext.TblWaterDosing.RemoveRange(_dbAquariumContext.TblWaterDosing.Where(aq => aq.AquariumId == aquariumId));
            _dbAquariumContext.TblWaterChange.RemoveRange(_dbAquariumContext.TblWaterChange.Where(aq => aq.AquariumId == aquariumId));
            _dbAquariumContext.TblAquariumPhoto.RemoveRange(_dbAquariumContext.TblAquariumPhoto.Where(aq => aq.AquariumId == aquariumId));
            _dbAquariumContext.TblAquariumEquipment.RemoveRange(aquarium.Equipment);

            if (aquarium.Device != null) DeleteAquariumDevice(aquarium.Device.Id);
            _dbAquariumContext.Remove(aquarium);
            _dbAquariumContext.SaveChanges();
        }

        /* Aquarium Snapshots */
        public AquariumSnapshot AddSnapshot(AquariumSnapshot model)
        {
            _dbAquariumContext.TblSnapshot.Add(model);
            _dbAquariumContext.SaveChanges();
            return model;
        }
        public List<AquariumSnapshot> GetSnapshotsByAquarium(int aquariumId)
        {
            return _dbAquariumContext.TblSnapshot.AsNoTracking()
                .Where(s => s.AquariumId == aquariumId)
                .Include(s => s.Photo).ToList();
        }
        public AquariumSnapshot GetSnapshotById(int id)
        {
            return _dbAquariumContext.TblSnapshot.AsNoTracking()
                .Include(s => s.Photo)
                .Include(s => s.Aquarium)
                .Where(snap => snap.Id == id).First();
        }
        public List<AquariumSnapshot> DeleteSnapshotsByAquarium(int aquariumId)
        {
            var snapshots = GetSnapshotsByAquarium(aquariumId);
            _dbAquariumContext.TblSnapshot.RemoveRange(snapshots);
            _dbAquariumContext.SaveChanges();

            var photoPath = _config["PhotoSubPath"] + aquariumId;
            //Check if photo folder exists
            if (Directory.Exists(photoPath))
                Directory.Delete(photoPath, true);
            return snapshots.ToList();

        }
        public AquariumSnapshot DeleteSnapshot(int snapshotId)
        {
            var snapshot = _dbAquariumContext.TblSnapshot.Single(e => e.Id == snapshotId);
            _dbAquariumContext.TblSnapshot.Remove(snapshot);
            _dbAquariumContext.SaveChanges();
            return snapshot;
        }
        [System.Obsolete]
        public List<AquariumSnapshot> GetAquariumSnapshots(int aquariumId, int offset, int max)
        {
            return _dbAquariumContext.TblSnapshot.Where(s => s.AquariumId == aquariumId)
                .Include(s => s.Photo)
                .OrderByDescending(s => s.StartTime)
                .Skip(offset)
                .Take(max)
                .ToList();
        }
        public void DeleteAllSnapshots(int aquariumId)
        {
            var snapshots = _dbAquariumContext.TblSnapshot.Where(s => s.AquariumId == aquariumId);
            _dbAquariumContext.RemoveRange(snapshots);
            _dbAquariumContext.SaveChanges();
        }
        public void DeleteSnapshots(List<int> snapshotIds)
        {
            var snapshots = _dbAquariumContext.TblSnapshot.Where(s => snapshotIds.Contains(s.Id));
            _dbAquariumContext.RemoveRange(snapshots);
            _dbAquariumContext.SaveChanges();
        }

        /* Aquarium Device */
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
                .Include(e => e.Sensors)
                .Include(d => d.ScheduleAssignments).ThenInclude(sa => sa.Schedule).ThenInclude(s => s.Tasks)
                .First();
            if (device.CameraConfiguration == null)
                device.CameraConfiguration = new CameraConfiguration();
            return device;
        }
        public AquariumDevice GetAquariumDeviceByIpAndKey(string ipAddress, string deviceKey)
        {
            if (ipAddress == "::1") ipAddress = "localhost";
            return _dbAquariumContext.TblDevice.AsNoTracking()
                .Where(s => s.Address == ipAddress && s.PrivateKey == deviceKey)
                .Include(e => e.Aquarium)
                .Include(e => e.CameraConfiguration)
                .First();
        }
        public AquariumDevice DeleteAquariumDevice(int deviceId)
        {
            var device = _dbAquariumContext.TblDevice
                .Include(e => e.CameraConfiguration)
                .Include(e => e.ScheduleAssignments)
                .ThenInclude(sa => sa.Schedule).ThenInclude(s => s.Tasks)
                .Include(e => e.Sensors)
                .SingleOrDefault(d => d.Id == deviceId);
            if (device == null)
                throw new KeyNotFoundException();
            _dbAquariumContext.TblDeviceATOStatus.RemoveRange(_dbAquariumContext.TblDeviceATOStatus.Where(aq => aq.DeviceId == deviceId));
            _dbAquariumContext.Remove(device);
            _dbAquariumContext.SaveChanges();
            return device;
        }
        public AquariumDevice UpdateAquariumDevice(AquariumDevice device)
        {
            var deviceToUpdate = _dbAquariumContext.TblDevice
                .Include(d => d.CameraConfiguration)
                .SingleOrDefault(s => s.Id == device.Id);
            if (deviceToUpdate == null)
                throw new KeyNotFoundException();

            _mapper.Map(device, deviceToUpdate);

            //Also map modules
            if (device.CameraConfiguration != null)
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
        public List<AquariumDevice> GetDevicesInUseBySchedule(int scheduleId)
        {
            var devices = _dbAquariumContext.TblDeviceScheduleAssignment
                .Where(sa => sa.ScheduleId == scheduleId)
                .Include(sa => sa.Device)
                .Select(sa => sa.Device)
                .ToList();
            return devices;
        }

        /* Aquarium Device Schedule */
        public DeviceSchedule AddDeviceSchedule(DeviceSchedule deviceSchedule)
        {
            _dbAquariumContext.Add(deviceSchedule);
            _dbAquariumContext.SaveChanges();
            return deviceSchedule;
        }
        public void UnassignDeviceSchedule(int scheduleId, int deviceId)
        {
            var res = _dbAquariumContext.TblDeviceScheduleAssignment.Where(sa => sa.DeviceId == deviceId && sa.ScheduleId == scheduleId);
            if (res.Any())
            {
                _dbAquariumContext.TblDeviceScheduleAssignment.RemoveRange(res);
                _dbAquariumContext.SaveChanges();
            }
        }
        public DeviceScheduleAssignment AssignDeviceSchedule(int scheduleId, int deviceId)
        {
            var res = _dbAquariumContext.TblDeviceScheduleAssignment.Where(sa => sa.DeviceId == deviceId && sa.ScheduleId == scheduleId);
            if (res.Any())
            {
                _dbAquariumContext.TblDeviceScheduleAssignment.RemoveRange(res);
            }
            var assignment = new DeviceScheduleAssignment()
            {
                DeviceId = deviceId,
                ScheduleId = scheduleId
            };
            _dbAquariumContext.TblDeviceScheduleAssignment.Add(assignment);
            _dbAquariumContext.SaveChanges();
            return assignment;
        }
        public List<DeviceSchedule> GetDeviceSchedulesByAccount(int accountId)
        {
            var deviceSchedules = _dbAquariumContext.TblDeviceSchedule
                .AsNoTracking()
                .Include(s => s.Tasks)
                .Where(s => s.AuthorId == accountId);
            return deviceSchedules.ToList();
        }
        public void DeleteDeviceSchedule(int scheduleId)
        {
            var scheduleAssignments = _dbAquariumContext.TblDeviceScheduleAssignment.Where(sa => sa.ScheduleId == scheduleId);
            _dbAquariumContext.TblDeviceScheduleAssignment.RemoveRange(scheduleAssignments);

            var scheduleTasks = _dbAquariumContext.TblDeviceScheduleTask.Where(st => st.ScheduleId == scheduleId);
            _dbAquariumContext.TblDeviceScheduleTask.RemoveRange(scheduleTasks);


            var schedule = _dbAquariumContext.TblDeviceSchedule.Where(s => s.Id == scheduleId).First();
            _dbAquariumContext.TblDeviceSchedule.Remove(schedule);

            _dbAquariumContext.SaveChanges();
        }
        public DeviceSchedule UpdateDeviceSchedule(DeviceSchedule deviceSchedule)
        {

            var scheduleToUpdate = _dbAquariumContext.TblDeviceSchedule
                .Include(d => d.Tasks)
                .First(s => s.Id == deviceSchedule.Id);

            //Truncate schedule tasks
            _dbAquariumContext.TblDeviceScheduleTask.RemoveRange(scheduleToUpdate.Tasks);

            _mapper.Map(deviceSchedule, scheduleToUpdate);

            _dbAquariumContext.SaveChanges();
            return deviceSchedule;
        }
        public List<DeviceScheduleAssignment> GetAssignedDeviceSchedules(int deviceId)
        {
            var schedules = _dbAquariumContext.TblDeviceScheduleAssignment
                .AsNoTracking()
                .Where(sa => sa.DeviceId == deviceId)
                .Include(sa => sa.Schedule).ThenInclude(s => s.Tasks)
                .ToList();
            return schedules;
        }
        public List<DeviceScheduleAssignment> GetScheduleAssignmentBySchedule(int scheduleId)
        {
            var schedules = _dbAquariumContext.TblDeviceScheduleAssignment
                .AsNoTracking()
                .Where(sa => sa.ScheduleId == scheduleId)
                .Include(sa => sa.Schedule).ThenInclude(s => s.Tasks)
                .ToList();
            return schedules;
        }

        /* Aquarium Photos */
        public AquariumPhoto GetAquariumPhotoById(int photoId)
        {
            return _dbAquariumContext.TblAquariumPhoto.AsNoTracking()
                .Where(p => p.Id == photoId)
                .First();
        }
        public IEnumerable<AquariumPhoto> GetAquariumPhotos(int aquariumId)
        {
            return _dbAquariumContext.TblAquariumPhoto.AsNoTracking()
                .Where(p => p.AquariumId == aquariumId)
                .Include(p => p.Photo);
        }
        public IEnumerable<FishPhoto> GetAquariumFishPhotos(int aquariumId)
        {
            return _dbAquariumContext.TblFishPhoto.AsNoTracking()
                .Include(f => f.Fish)
                .Include(f => f.Photo)
                .Where(p => p.Fish.AquariumId == aquariumId);
        }
        public List<AquariumPhoto> GetAquariumPhotosByAccount(int accountId)
        {
            return _dbAquariumContext.TblAquariumPhoto.AsNoTracking()
                .Include(p => p.Photo)
                .Include(p => p.Aquarium)
                .Where(p => p.Aquarium.OwnerId == accountId)
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
            _dbAquariumContext.TblAquariumPhoto.Remove(photo);
            _dbAquariumContext.SaveChanges();
        }
        public void DeleteAquariumPhotos(List<int> photoIds)
        {
            var photos = _dbAquariumContext.TblAquariumPhoto.Where(s => photoIds.Contains(s.Id))
                .Include(p => p.Photo);
            _dbAquariumContext.RemoveRange(photos);
            _dbAquariumContext.SaveChanges();
        }
        public void DeleteFishPhoto(int photoId)
        {
            var photo = GetFishPhotoById(photoId);
            _dbAquariumContext.TblFishPhoto.Remove(photo);
            _dbAquariumContext.SaveChanges();
        }

        public List<PhotoContent> GetPhotoContentByIds(int[] photoIds)
        {
            return _dbAquariumContext.TblPhotoContent.AsNoTracking()
                .Where(p => photoIds.Contains(p.Id))
                .ToList();
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
                .Where(s => s.Id == fishId)
                .Include(f => f.Species)
                .Include(f => f.Aquarium)
                .Include(f => f.Feedings)
                .Include(f => f.Thumbnail).ThenInclude(p => p.Photo)
                .Include(f => f.Death)
                .Include(f => f.Disease).ThenInclude(d => d.Treatment)
                .Include(f => f.Photos).ThenInclude(p => p.Photo)
                .FirstOrDefault();
        }
        public Fish AddFish(Fish fish)
        {
            fish.Species = null;
            fish.Aquarium = null;
            _dbAquariumContext.TblFish.Add(fish);
            _dbAquariumContext.SaveChanges();
            return fish;
        }
        public Fish UpdateFish(Fish fish)
        {
            var fishToUpdate = GetFishById(fish.Id);
            if (fishToUpdate == null)
                throw new KeyNotFoundException();
            if (fishToUpdate.AquariumId != fish.AquariumId)
                throw new Exception("AquariumId cannot be changed while updating entries");

            _mapper.Map(fish, fishToUpdate);
            _dbAquariumContext.TblFish.Update(fishToUpdate);
            _dbAquariumContext.SaveChanges();
            return GetFishById(fishToUpdate.Id);
        }
        public void DeleteFish(int fishId)
        {
            var fishToUpdate = _dbAquariumContext.TblFish.Where(s => s.Id == fishId).First();
            if (fishToUpdate == null)
                throw new KeyNotFoundException();
            _dbAquariumContext.TblFish.Remove(fishToUpdate);
            _dbAquariumContext.SaveChanges();
        }
        public List<Fish> GetAllFishByAccount(int accountId)
        {
            return _dbAquariumContext.TblFish.Where(f => f.Aquarium.OwnerId == accountId).ToList();
        }
        public Fish TransferFish(int fishId, int aquariumId)
        {
            var fish = _dbAquariumContext.TblFish.Where(f => f.Id == fishId).First();
            fish.AquariumId = aquariumId;
            _dbAquariumContext.Update(fish);
            _dbAquariumContext.SaveChanges();
            return fish;
        }
        public Fish MarkDeseased(FishDeath death)
        {
            var fish = _dbAquariumContext.TblFish.Where(f => f.Id == death.FishId).First();
            fish.Death = death;
            fish.Dead = true;
            _dbAquariumContext.Update(fish);
            _dbAquariumContext.SaveChanges();
            return GetFishById(death.FishId);
        }
        public FishBreeding AddBreeding(FishBreeding breeding)
        {
            _dbAquariumContext.TblFishBreeds.Add(breeding);
            _dbAquariumContext.SaveChanges();

            var mother = _dbAquariumContext.TblFish.AsNoTracking()
                .Where(f => f.Id == breeding.MotherId)
                .Include(f => f.Species)
                .First();

            var newFish = new List<Fish>();

            for (var i = 0; i < breeding.Amount; i++)
            {
                newFish.Add(new Fish()
                {
                    AquariumId = mother.AquariumId,
                    BreedId = breeding.Id,
                    SpeciesId = mother.SpeciesId,
                    Name = mother.Species.Name + $"({i})"
                });
            }
            _dbAquariumContext.UpdateRange(newFish);
            _dbAquariumContext.SaveChanges();
            breeding.Fish = newFish;
            return breeding;
        }
        public FishDisease AddDiseaseDiagnosis(FishDisease diseaseDiagnois)
        {
            _dbAquariumContext.Add(diseaseDiagnois);
            _dbAquariumContext.SaveChanges();
            return diseaseDiagnois;
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

        /* Posts */
        public List<PostCategory> GetPostCategories()
        {
            var categories  = _dbAquariumContext.TblPostCategories
                .Include(c => c.Boards)
                .ToList();

            categories.ForEach(c =>
            {
                c.Boards.ToList().ForEach(b =>
                {
                    var data = _dbAquariumContext.vwPostBoards.Where(v => v.Id == b.Id).First();
                    b.PostCount = data.PostCount;
                    b.ThreadCount = data.ThreadCount;
                });
            });
            return categories;
        }
        public PostBoard GetBoardById(int boardId)
        {
            var board = _dbAquariumContext.TblPostBoards
                .Where(b => b.Id == boardId)
                .Include(c => c.Threads).ThenInclude(t => t.Author)
                .Include(b => b.Category)
                .First();
            var threads = board.Threads.ToList();
            threads.ForEach(t =>
            {
                t.PostCount = _dbAquariumContext.TblPosts.Where(p => p.ThreadId == t.Id).Count();
            });
            board.Threads = threads;
            return board;
        }
        public PostThread GetThreadById(int threadId)
        {
            var thread = _dbAquariumContext.TblPostThreads.Where(t => t.Id == threadId)
                .Include(t => t.Board).ThenInclude(b => b.Category)
                .First();

            thread.Posts = _dbAquariumContext.TblPosts.Where(p => p.ThreadId == threadId)
                .Include(p => p.Author).ThenInclude(p => p.Profile)
                .ToList();
            return thread;
        }
        public Post GetPostById(int postId)
        {
            return _dbAquariumContext.TblPosts.Where(p => p.Id == postId)
                .Include(p => p.Author).ThenInclude(a => a.Profile)
                .Include(p => p.Thread)
                    .ThenInclude(t => t.Board)
                       .ThenInclude(t => t.Category)
                .First();
        }
        public PostCategory CreatePostCategory(PostCategory category)
        {
            _dbAquariumContext.TblPostCategories.Update(category);
            _dbAquariumContext.SaveChanges();
            return category;
        }
        public PostBoard CreatePostBoard(PostBoard board)
        {
            _dbAquariumContext.TblPostBoards.Add(board);
            _dbAquariumContext.SaveChanges();
            return board;
        }
        public PostThread CreatePostThread(PostThread thread)
        {
            _dbAquariumContext.TblPostThreads.Add(thread);
            _dbAquariumContext.SaveChanges();
            return thread;
        }
        public Post CreatePost(Post post)
        {
            _dbAquariumContext.TblPosts.Add(post);
            _dbAquariumContext.SaveChanges();
            return post;
        }
        public void DeletePostCategory(int categoryId)
        {
            var category = _dbAquariumContext.TblPostCategories.Where(c => c.Id == categoryId)
                .Include(c => c.Boards).ThenInclude(b =>  b.Threads).ThenInclude(t => t.Posts)
                .First();

            foreach(var board in category.Boards)
            {
                foreach (var thread in board.Threads)
                    _dbAquariumContext.TblPosts.RemoveRange(thread.Posts);
                _dbAquariumContext.TblPostThreads.RemoveRange(board.Threads);
            }
            _dbAquariumContext.TblPostBoards.RemoveRange(category.Boards);
            _dbAquariumContext.TblPostCategories.Remove(category);
            _dbAquariumContext.SaveChanges();
        }
        public void DeletePostBoard(int boardId)
        {
            var board = _dbAquariumContext.TblPostBoards.Where(c => c.Id == boardId)
                .Include(b  => b.Threads).ThenInclude(t => t.Posts)
                .First();

            foreach (var thread in board.Threads)
                _dbAquariumContext.TblPosts.RemoveRange(thread.Posts);
            _dbAquariumContext.TblPostThreads.RemoveRange(board.Threads);
            _dbAquariumContext.TblPostBoards.Remove(board);
        }
        public void DeletePostThread(int threadId)
        {
            var thread = _dbAquariumContext.TblPostThreads.Where(c => c.Id == threadId)
                .Include(t => t.Posts)
                .First();
            _dbAquariumContext.TblPosts.RemoveRange(thread.Posts);
            _dbAquariumContext.TblPostThreads.Remove(thread);
        }
        public void DeletePost(int postId)
        {
            var post = _dbAquariumContext.TblPosts.Where(c => c.Id == postId).First();
            _dbAquariumContext.TblPosts.Remove(post);
        }

        /* Photo Content */
        public PhotoContent CreatePhotoReference()
        {
            var content = new PhotoContent();
            _dbAquariumContext.TblPhotoContent.Add(content);
            _dbAquariumContext.SaveChanges();
            return content;
        }
        public PhotoContent UpdatePhotoReference(PhotoContent content)
        {
            _dbAquariumContext.TblPhotoContent.Update(content);
            _dbAquariumContext.SaveChanges();
            return content;
        }
        public void DeletePhoto(int photoId)
        {
            var photo = _dbAquariumContext.TblPhotoContent.Where(p => p.Id == photoId).First();
            _dbAquariumContext.TblPhotoContent.Remove(photo);

            var fishPhoto = _dbAquariumContext.TblFishPhoto.Where(p => p.PhotoId == photoId).FirstOrDefault();
            if (fishPhoto != null)
                _dbAquariumContext.TblFishPhoto.Remove(fishPhoto);

            var aquariumPhoto = _dbAquariumContext.TblAquariumPhoto.Where(p => p.PhotoId == photoId).FirstOrDefault();
            if (aquariumPhoto != null)
                _dbAquariumContext.TblAquariumPhoto.Remove(aquariumPhoto);

            _dbAquariumContext.SaveChanges();
        }
        public PhotoContent GetPhoto(int photoId)
        {
            var photo = _dbAquariumContext.TblPhotoContent.AsNoTracking()
                .Where(p => p.Id == photoId).First();
            return photo;
        }
        public FishPhoto AddFishPhoto(FishPhoto photo)
        {
            _dbAquariumContext.TblFishPhoto.Add(photo);
            _dbAquariumContext.SaveChanges();
            return photo;
        }
        public FishPhoto GetFishPhotoById(int photoId)
        {
            return _dbAquariumContext.TblFishPhoto.AsNoTracking()
                .Where(p => p.Id == photoId)
                .First();
        }


        /* Misc. */
        public BugReport AddBugReport(BugReport report)
        {
            _dbAquariumContext.TblBugReports.Add(report);
            _dbAquariumContext.SaveChanges();
            return report;
        }
        public List<BugReport> GetAllBugs()
        {
            var bugs = _dbAquariumContext.TblBugReports.AsNoTracking()
                .Include(r => r.ImpactedUser)
                .ToList();
            return bugs;
        }
        public List<AquariumSnapshot> GetAquariumTemperatureHistogram(int aquariumId)
        {
            return _dbAquariumContext.TblSnapshot.AsNoTracking()
                .Where(s => s.AquariumId == aquariumId && s.Temperature != null)
                .ToList();
        }
        public List<SearchResult> PerformSearch(SearchOptions options)
        {
            var results = new List<SearchResult>();
            if (options.Accounts)
                results.AddRange(_dbAquariumContext.TblAccounts.Where(a =>
                a.Username.Contains(options.Query) ||
                a.Email.Contains(options.Query)
               ).ToList().Select(r => new SearchResult()
               {
                   Type = "Account",
                   Data = new AquariumProfile()
                   {
                       Account = r,
                       Relationship = GetAccountRelationship(options.AccountId, r.Id)
                   }
               }));
            if (options.Aquariums)
                results.AddRange(_dbAquariumContext.TblAquarium.Where(a =>
                a.Name.Contains(options.Query)
               ).Select(r => new SearchResult() { Type = "Aquarium", Data = r }));
            if (options.Fish)
                results.AddRange(_dbAquariumContext.TblFish.Where(a =>
                a.Name.Contains(options.Query)
               ).Select(r => new SearchResult() { Type = "Fish", Data = r }));
            if (options.Species)
                results.AddRange(_dbAquariumContext.TblSpecies.Where(a =>
                a.Name.Contains(options.Query)
               ).Select(r => new SearchResult() { Type = "Species", Data = r }));
            if (options.Posts)
                results.AddRange(_dbAquariumContext.TblPosts.Where(a =>
                a.Title.Contains(options.Query) ||
                a.Content.Contains(options.Query)
               ).Select(r => new SearchResult() { Type = "Post", Data = r }));
            if (options.Threads)
                results.AddRange(_dbAquariumContext.TblPostThreads.Where(a =>
                a.Title.Contains(options.Query)
               ).Select(r => new SearchResult() { Type = "Thread", Data = r }));

            return results.Take(10).ToList();
        }

        public void DeleteDispatchedNotification(int notificationId)
        {
            DeleteDispatchedNotifications(new List<int>() { notificationId });
        }
        public void DeleteDispatchedNotifications(List<int> notificationIds)
        {
            var affectedNotifications = _dbAquariumContext.TblDispatchedNotifications
                .Include(n => n.Notifications)
                .Where(n => notificationIds.Contains(n.Id));
            _dbAquariumContext.RemoveRange(affectedNotifications);
            _dbAquariumContext.SaveChanges();
        }

        public void DismissDispatchedNotification(int notificationId)
        {
            DismissDispatchedNotifications(new List<int>() { notificationId });
        }
        public void DismissDispatchedNotifications(List<int> notificationIds)
        {
            var affectedNotifications = _dbAquariumContext.TblNotification.Where(n => notificationIds.Contains(n.SourceId)).ToList();
            affectedNotifications.ForEach(n => n.Dismissed = true);
            _dbAquariumContext.UpdateRange(affectedNotifications);
            _dbAquariumContext.SaveChanges();
        }

        public void DeleteNotification(int notificationId)
        {
            var notif = _dbAquariumContext.TblNotification.Where(n => n.Id == notificationId)
                .First();
            _dbAquariumContext.Remove(notif);
            _dbAquariumContext.SaveChanges();
        }

        public void DismissNotifications(List<int> notificationIds)
        {
            var affectedNotifications = _dbAquariumContext.TblNotification.Where(n => notificationIds.Contains(n.Id)).ToList();
            affectedNotifications.ForEach(n => n.Dismissed = true);
            _dbAquariumContext.UpdateRange(affectedNotifications);
            _dbAquariumContext.SaveChanges();
        }

        public async Task EmitNotification(DispatchedNotification notif)
        {
            _dbAquariumContext.Add(notif);
            _dbAquariumContext.SaveChanges();
            _dbAquariumContext.TblAccounts.ToList().ForEach(u =>
            {
                _dbAquariumContext.Add(new Notification
                {
                    SourceId = notif.Id,
                    TargetId = u.Id
                });
            });
            await _dbAquariumContext.SaveChangesAsync();
        }

        public async Task EmitNotification(DispatchedNotification notif, List<int> aquariumAccountIds)
        {
            _dbAquariumContext.Add(notif);
            _dbAquariumContext.SaveChanges();
            aquariumAccountIds.ForEach(id =>
            {
                _dbAquariumContext.Add(new Notification
                {
                    SourceId = notif.Id,
                    TargetId = id
                });
            });
            await _dbAquariumContext.SaveChangesAsync();
        }

        public ICollection<DispatchedNotification> GetAllDispatchedNotifications()
        {
            return _dbAquariumContext.TblDispatchedNotifications
                .AsNoTracking()
                .Include(n => n.Notifications)
                .Include(n => n.Dispatcher)
                .ToList();
        }

        public ICollection<Notification> GetNotificationsByAccountId(int id)
        {
            return _dbAquariumContext.TblNotification
                .AsNoTracking()
                .Include(n => n.Source).ThenInclude(n => n.Dispatcher)
                .Include(n => n.Target)
                //.ThenInclude(n => n.Dispatcher)
                .Where(n => n.TargetId == id)
                .Take(20)
                .OrderByDescending(n => n.Source.Date)
                .ToList();
        }
        public ICollection<Notification> GetNotificationsById(List<int> notificationIds)
        {
            return _dbAquariumContext.TblNotification
                .AsNoTracking()
                .Include(n => n.Source)
                //.ThenInclude(n => n.Dispatcher)
                .Where(n => notificationIds.Contains(n.Id))
                .ToList();
        }
        public ICollection<AquariumSnapshot> GetWaterParametersByAquarium(int aquariumId,PaginationSliver pagination)
        {
            var range = _dbAquariumContext.TblSnapshot
                .AsNoTracking()
                .Where(s => s.AquariumId == aquariumId).ApplyPaginationSliver<AquariumSnapshot>(pagination);
            var list = range.ToList();
            return list;
        }
        public ICollection<WaterChange> GetWaterChangesByAquarium(int aquariumId)
        {
            var waterChanges = _dbAquariumContext.TblWaterChange.Where(n => n.AquariumId == aquariumId).ToList();
            return waterChanges;
        }
        public List<ATOStatus> GetWaterATOByAquarium(int aquariumId, PaginationSliver pagination)
        {
            var range = _dbAquariumContext.TblDeviceATOStatus
                .AsNoTracking()
                .Where(s => s.AquariumId == aquariumId).ApplyPaginationSliver<ATOStatus>(pagination);
            var list = range.ToList();
            return list;
        }
        public WaterChange AddWaterChange(WaterChange waterChange)
        {
            _dbAquariumContext.TblWaterChange.Add(waterChange);
            _dbAquariumContext.SaveChanges();
            return waterChange;
        }
        public WaterChange UpdateWaterChange(WaterChange waterChange)
        {
            _dbAquariumContext.TblWaterChange.Update(waterChange);
            _dbAquariumContext.SaveChanges();
            return waterChange;
        }
        public void DeleteWaterChanges(List<int> waterChangeIds)
        {
            var removedWaterChanges = _dbAquariumContext.TblWaterChange.Where(n => waterChangeIds.Contains(n.Id)).ToList();
            _dbAquariumContext.RemoveRange(removedWaterChanges);
            _dbAquariumContext.SaveChanges();
        }
        public ICollection<WaterDosing> GetWaterDosingsByAquarium(int aquariumId)
        {
            var waterDosings = _dbAquariumContext.TblWaterDosing.Where(n => n.AquariumId == aquariumId).ToList();
            return waterDosings;
        }
        public WaterDosing AddWaterDosing(WaterDosing waterDosing)
        {
            _dbAquariumContext.TblWaterDosing.Add(waterDosing);
            _dbAquariumContext.SaveChanges();
            return waterDosing;
        }
        public WaterDosing UpdateWaterDosing(WaterDosing waterDosing)
        {
            _dbAquariumContext.TblWaterDosing.Update(waterDosing);
            _dbAquariumContext.SaveChanges();
            return waterDosing;
        }
        public void DeleteWaterDosings(List<int> waterDosingIds)
        {
            var removedWaterDosings = _dbAquariumContext.TblWaterDosing.Where(n => waterDosingIds.Contains(n.Id)).ToList();
            _dbAquariumContext.RemoveRange(removedWaterDosings);
            _dbAquariumContext.SaveChanges();
        }

        public ICollection<AquariumSnapshot> GetSnapshotsByIds(List<int> snapshotIds)
        {
            return _dbAquariumContext.TblSnapshot.Where(s => snapshotIds.Contains(s.Id)).ToList();
        }





        /* Device Sensors */
        public DeviceSensor AddDeviceSensor(DeviceSensor deviceSensor)
        {
            _dbAquariumContext.TblDeviceSensor.Add(deviceSensor);
            _dbAquariumContext.SaveChanges();
            return deviceSensor;
        }
        public ICollection<DeviceSensor> GetDeviceSensors(int deviceId)
        {
            var deviceSensors = _dbAquariumContext.TblDeviceSensor.Where(n => n.DeviceId == deviceId).ToList();
            return deviceSensors;
        }
        public DeviceSensor UpdateDeviceSensor(DeviceSensor deviceSensor)
        {
            _dbAquariumContext.TblDeviceSensor.Update(deviceSensor);
            _dbAquariumContext.SaveChanges();
            return deviceSensor;
        }
        public void DeleteDeviceSensors(List<int> deviceSensorIds)
        {
            var removedDeviceSensors = _dbAquariumContext.TblDeviceSensor.Where(n => deviceSensorIds.Contains(n.Id)).ToList();
            _dbAquariumContext.RemoveRange(removedDeviceSensors);
            _dbAquariumContext.SaveChanges();
        }



        public ATOStatus AddATOStatus(ATOStatus atoStatus)
        {
            _dbAquariumContext.TblDeviceATOStatus.Add(atoStatus);
            _dbAquariumContext.SaveChanges();
            return atoStatus;
        }
        public ATOStatus UpdateATOStatus(ATOStatus atoStatus)
        {
            _dbAquariumContext.TblDeviceATOStatus.Update(atoStatus);
            _dbAquariumContext.SaveChanges();
            return atoStatus;
        }
        [System.Obsolete]
        public List<ATOStatus> GetATOHistory(int deviceId,PaginationSliver pagination)
        {
            var range = _dbAquariumContext.TblDeviceATOStatus
                .AsNoTracking()
                .Where(s => s.DeviceId == deviceId).ApplyPaginationSliver<ATOStatus>(pagination);
            var list = range.ToList();
            return list;
        }
    }
}

