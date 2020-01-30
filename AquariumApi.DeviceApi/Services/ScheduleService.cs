using AquariumApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public class ScheduleService : BackgroundService
    {
        private readonly ILogger<ScheduleService> _logger;
        private readonly IDeviceService _deviceService;
        private readonly IConfiguration _config;

        public CancellationToken token;
        public bool Running;

        public List<DeviceSchedule> _schedules { get; private set; } = new List<DeviceSchedule>();

        public ScheduleService(IConfiguration config, ILogger<ScheduleService> logger, IDeviceService deviceService)
        {
            _config = config;
            _logger = logger;
            _deviceService = deviceService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            token = stoppingToken;
            //stoppingToken.Register(() => Cleanup());

            _logger.LogInformation($"Schedule job starting...");
            _schedules = LoadSchedulesFromCache();
            _logger.LogInformation($"{_schedules.Count} schedules have been found");
            try
            {
                Running = true;

                DateTime? lastTokenRenewalTime = null;

                while (!stoppingToken.IsCancellationRequested)
                {
                    //Check if we should renew token
                    if(lastTokenRenewalTime == null || DateTime.Now - lastTokenRenewalTime > TimeSpan.FromDays(1))
                    {
                        lastTokenRenewalTime = DateTime.Now;
                        await _deviceService.RenewAuthenticationToken();
                    }

                    var task = GetNextTask(_schedules, DateTime.Now);
                    if (task != null)
                    {
                        _logger.LogInformation($"Next task scheduled in {task.eta.TotalMinutes} minutes (Schedule: {task.task.Schedule.Name})");
                        await Task.Delay(task.eta, stoppingToken);
                        try
                        {
                            _logger.LogInformation($"\n\n ** Performing task (TaskId: {task.task.TaskId} Schedule: {task.task.Schedule.Name}) **");
                            PerformTask(task.task);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Could not perform task [taskId:{task.task.TaskId} Schedule: {task.task.Schedule.Name}]: Error: {e.Message}");
                            _logger.LogError(e.StackTrace);
                        }
                        _logger.LogInformation($"\n\n ** Task completed successfully (TaskId: {task.task.TaskId} Schedule: {task.task.Schedule.Name}) **");
                    }
                    else
                    {
                        _logger.LogInformation("We have no tasks!");
                        break;
                    }
                }
            }
            catch (TaskCanceledException e)
            {
                //do nothing, it was canceled
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured during schedule: {e.Message}");
            }
            Cleanup();
        }

        private void Cleanup()
        {
            _logger.LogInformation($"Schedule job stopped");
            Running = false;
        }

        public void SaveSchedulesToCache(List<DeviceSchedule> deviceSchedules)
        {
            var filepath = _config["ScheduleAssignmentPath"];
            var json = JsonConvert.SerializeObject(deviceSchedules);
            System.IO.File.WriteAllText(filepath, json);
            _logger.LogWarning($"Schedule assignments saved ({deviceSchedules.Count} schedules written)");
        }
        private List<DeviceSchedule> LoadSchedulesFromCache()
        {
            var filepath = _config["ScheduleAssignmentPath"];
            try
            {
                var json = System.IO.File.ReadAllText(filepath);
                return JsonConvert.DeserializeObject<List<DeviceSchedule>>(json);
            }
            catch (FileNotFoundException e)
            {
                return new List<DeviceSchedule>();
            }
        }

        public List<DeviceSchedule> GetAllSchedules()
        {
            return _schedules;
        }
        public ScheduleState GetStatus()
        {
            var nextTask = GetNextTask(_schedules, DateTime.Now);
            return new ScheduleState
            {
                Running = Running,
                NextTask = nextTask,
                Schedules = _schedules,
                TaskCount = _schedules.SelectMany(s => s.ExpandTasks()).Count()
            };
        }
        public FutureTask GetNextTask(List<DeviceSchedule> schedules, DateTime? currentTime = null)
        {
            var now = DateTime.Now;
            if (currentTime != null)
                now = currentTime.Value;

            var allScheduledTasks = schedules.SelectMany(s => s.ExpandTasks()).ToList();

            if (allScheduledTasks.Count() == 0)
                return null;




            var remainingTasks = allScheduledTasks.Where(task =>
            {
                var taskTime = task.StartTime.TimeOfDay;
                var nowTime = now.TimeOfDay;
                if (taskTime > nowTime)
                    return true;
                return false;
            }).OrderBy(t => t.StartTime);

            var nextTask = remainingTasks.FirstOrDefault();


            //No tasks left for today, check tomorrow
            if (nextTask == null)
            {
                var firstTask = allScheduledTasks[0];
                var eta = firstTask.StartTime.AddDays(1).Subtract(now.TimeOfDay).TimeOfDay;
                if (allScheduledTasks.Count() > 0)

                    return new FutureTask
                    {
                        Index = 0,
                        task = firstTask,
                        eta = eta
                    };
                else
                    return null;
            }



            return new FutureTask
            {
                Index = allScheduledTasks.IndexOf(nextTask),
                task = nextTask,
                eta = nextTask.StartTime.TimeOfDay.Subtract(now.TimeOfDay)
            };
        }



        /* Tasks */

        public void PerformTask(DeviceScheduleTask task)
        {
            switch (task.TaskId)
            {
                case ScheduleTaskTypes.Snapshot:
                    TakeSnapshotTask(task);
                    break;
                default:
                    _logger.LogError($"Invalid task type id (taskId: {task.Id})");
                    break;
            }
        }
        private void TakeSnapshotTask(DeviceScheduleTask task)
        {
            _logger.LogInformation("Taking aquarium snapshot...");
            var cameraConfiguration = _deviceService.GetConnectionInformation().Aquarium.Device.CameraConfiguration;
            var snapshot = _deviceService.TakeSnapshot();
            var photo = _deviceService.TakePhoto(cameraConfiguration);

            _deviceService.SendAquariumSnapshotToHost(snapshot, photo);
            _logger.LogInformation("Aquarium snapshot sent successfully");
        }

    }
}














