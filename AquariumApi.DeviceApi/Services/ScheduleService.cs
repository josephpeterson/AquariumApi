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

        public List<DeviceSchedule> _schedules { get; private set; }

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

            _logger.LogDebug($"Schedule job starting...");
            _schedules = LoadSchedulesFromCache();
            _logger.LogInformation($"{_schedules.Count} schedules have been found");
            try
            {
                Running = true;
                while (!stoppingToken.IsCancellationRequested)
                {
                    var task = GetNextTask(_schedules, DateTime.Now);
                    if (task != null)
                    {
                        _logger.LogInformation($"Next task scheduled in {task.eta.TotalMinutes} minutes (Schedule: {task.task.Schedule.Name})");
                        await Task.Delay(task.eta, stoppingToken);
                        try
                        {
                            _logger.LogInformation($"Performing task (TaskId: {task.task.TaskId} Schedule: {task.task.Schedule.Name}");
                            PerformTask(task.task);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Could not perform task [taskId:{task.task.TaskId} Schedule: {task.task.Schedule.Name}]: Error: {e.Message}");
                            _logger.LogError(e.StackTrace);
                        }
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
            _logger.LogDebug($"Schedule job stopped");
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
            return new ScheduleState
            {
                Running = Running,
                NextTask = GetNextTask(_schedules, DateTime.Now),
                Schedules = _schedules
            };
        }
        public FutureTask GetNextTask(List<DeviceSchedule> schedules, DateTime? currentTime = null)
        {
            var now = DateTime.Now.TimeOfDay;
            if (currentTime != null)
                now = currentTime.Value.TimeOfDay;

            var allScheduledTasks = schedules.SelectMany(s => s.ExpandTasks());


            var remainingTasks = allScheduledTasks.Where(task =>
            {
                var taskTime = task.StartTime.TimeOfDay;
                return taskTime > now;
            });

            var nextTask = remainingTasks.FirstOrDefault();
            if (nextTask == null)
                return null;
            return new FutureTask
            {
                task = nextTask,
                eta = nextTask.StartTime.TimeOfDay.Subtract(now)
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
            var device = _deviceService.GetDevice();
            var snapshot = _deviceService.TakeSnapshot();
            var photo = _deviceService.TakePhoto(device.CameraConfiguration);
            var deviceId = task.Schedule.ScheduleAssignments
                        .Where(sa => sa.ScheduleId == task.ScheduleId)
                        .Select(sa => sa.DeviceId)
                        .First();

            _deviceService.SendAquariumSnapshotToHost(task.Schedule.Host, deviceId, snapshot, photo);
            _logger.LogInformation("Aquarium snapshot sent successfully");
        }

    }
}














