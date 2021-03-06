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
    public class ScheduleService : BackgroundService, IDeviceSetupService
    {
        private readonly ILogger<ScheduleService> _logger;
        private IAquariumAuthService _aquariumAuthService;
        private readonly IConfiguration _config;
        private Dictionary<ScheduleTaskTypes, DeviceTaskProcess> _deviceTaskCallbacks = new Dictionary<ScheduleTaskTypes, DeviceTaskProcess>();
        public CancellationToken token;
        public bool Running;


        public ScheduleService(IConfiguration config, ILogger<ScheduleService> logger,IAquariumAuthService aquariumAuthService)
        {
            _config = config;
            _logger = logger;
            _aquariumAuthService = aquariumAuthService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            token = stoppingToken;
            //stoppingToken.Register(() => Cleanup());

            //_schedules = LoadSchedulesFromCache();
            DeviceScheduleTask task;
            try
            {
                Running = true;
                while (!stoppingToken.IsCancellationRequested)
                {
                    task = GetNextTask();
                    if (task != null)
                    {
                        var eta = task.GetTaskETA();

                        //readable time
                        string time = string.Format("{0:D2}h:{1:D2}m",eta.Hours,eta.Minutes);
                        var schedule = GetAllSchedules().Where(s => s.Id == task.ScheduleId).FirstOrDefault();
                        var scheduleName = schedule == null ? "Unknown" : schedule.Name;
                        _logger.LogInformation($"Next task scheduled in {time} (Schedule: {scheduleName} Task: {task.TaskId})");
                        await Task.Delay(eta, stoppingToken);
                        try
                        {
                            _logger.LogInformation($"\n\n ** Performing task (TaskId: {task.TaskId} Schedule: {scheduleName}) **");
                            PerformTask(task);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Could not perform task [taskId:{task.TaskId} Schedule: {scheduleName}]: Error: {e.Message}");
                            _logger.LogError(e.StackTrace);
                        }
                        _logger.LogInformation($"\n\n ** Task completed successfully (TaskId: {task.TaskId} Schedule: {scheduleName}) **");
                    }
                    else
                    {
                        _logger.LogInformation("We have no tasks!");
                        break;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                //do nothing, it was canceled
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured during schedule: {e.Message}");
                _logger.LogError($"{e.StackTrace}");
            }
            _logger.LogInformation($"Schedule job stopped");
            CleanUp();
        }

        public void SaveSchedulesToCache()
        {
            var deviceSchedules = GetAllSchedules();
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
            catch (FileNotFoundException)
            {
                return new List<DeviceSchedule>();
            }
        }

        public List<DeviceSchedule> GetAllSchedules()
        {
            var aq = _aquariumAuthService.GetAquarium();
            if(aq == null || aq.Device == null)
                return new List<DeviceSchedule>();
            var sa = aq.Device.ScheduleAssignments;
            if (sa == null)
                return new List<DeviceSchedule>();
            return sa.Select(s => s.Schedule).ToList();
        }
        public ScheduleState GetStatus()
        {
            var nextTask = GetNextTask(DateTime.Now);
            return new ScheduleState
            {
                Running = Running,
                NextTask = nextTask,
                Schedules = GetAllSchedules(),
                TaskCount = GetAllScheduledTasks().Count(),
                ScheduledTasks = GetAllScheduledTasks()
            };
        }
        public DeviceScheduleTask GetNextTask(DateTime? currentTime = null)
        {
            return GetAllScheduledTasks().FirstOrDefault();
        }


        public List<DeviceScheduleTask> GetAllScheduledTasks()
        {
            var now = DateTime.UtcNow;
            var schedules = GetAllSchedules();
            return schedules.SelectMany(s => s.ExpandTasks(now)).ToList();
            
        }
        public void Setup()
        {
            var schedules = GetAllSchedules();
            if(schedules.Count() == 0)
            {
                _logger.LogInformation("No schedules are deployed on this device.");
                return;

            }

            _logger.LogInformation($"{schedules.Count()} Schedules found");
            SaveSchedulesToCache();
            StopAsync(token).Wait();
            StartAsync(new System.Threading.CancellationToken()).Wait();
        }

        /* Tasks */

        public void PerformTask(DeviceScheduleTask task)
        {
            if (_deviceTaskCallbacks.ContainsKey(task.TaskId))
                _deviceTaskCallbacks[task.TaskId](task);
            else
                throw new Exception($"Invalid task type id (taskId: {task.Id})");
        }
        public void RegisterDeviceTask(ScheduleTaskTypes taskType,DeviceTaskProcess callback)
        {
            _deviceTaskCallbacks[taskType] = callback;
            _logger.LogInformation($"Registered callback for task id: {taskType}");
        }

        public void CleanUp()
        {
            StopAsync(token).Wait();
            Running = false;
        }

        public delegate void DeviceTaskProcess(DeviceScheduleTask task);
    }
}














