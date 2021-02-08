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
        private readonly IATOService _atoService;
        private readonly IConfiguration _config;
        private AquariumDevice _device = new AquariumDevice();
        public CancellationToken token;
        public bool Running;

        public ScheduleService(IConfiguration config, ILogger<ScheduleService> logger, IDeviceService deviceService,IATOService atoService)
        {
            _config = config;
            _logger = logger;
            _deviceService = deviceService;
            _atoService = atoService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            token = stoppingToken;
            //stoppingToken.Register(() => Cleanup());

            //_schedules = LoadSchedulesFromCache();
            try
            {
                Running = true;

                DateTime? lastTokenRenewalTime = null;

                while (!stoppingToken.IsCancellationRequested)
                {
                    //Check if we should renew token
                    //todo: move this into its own background service
                    if(lastTokenRenewalTime == null || DateTime.Now - lastTokenRenewalTime > TimeSpan.FromDays(1))
                    {
                        lastTokenRenewalTime = DateTime.Now;
                        await _deviceService.RenewAuthenticationToken();
                    }

                    var task = GetNextTask();
                    if (task != null)
                    {
                        var eta = task.GetTaskETA();
                        _logger.LogInformation($"Next task scheduled in {Math.Ceiling(eta.TotalMinutes)} minutes (Schedule: {task.Schedule.Name})");
                        await Task.Delay(eta, stoppingToken);
                        try
                        {
                            _logger.LogInformation($"\n\n ** Performing task (TaskId: {task.TaskId} Schedule: {task.Schedule.Name}) **");
                            PerformTask(task);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Could not perform task [taskId:{task.TaskId} Schedule: {task.Schedule.Name}]: Error: {e.Message}");
                            _logger.LogError(e.StackTrace);
                        }
                        _logger.LogInformation($"\n\n ** Task completed successfully (TaskId: {task.TaskId} Schedule: {task.Schedule.Name}) **");
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
            catch (FileNotFoundException e)
            {
                return new List<DeviceSchedule>();
            }
        }

        public List<DeviceSchedule> GetAllSchedules()
        {
            var sa = _device.ScheduleAssignments;
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


            var allScheduledTasks = new List<DeviceScheduleTask>();
            schedules.ForEach(s => s.Tasks.ToList().ForEach(t =>
            {
                var tod = t.StartTime.TimeOfDay;
                var startTime = now.Date + tod;
                if (startTime < now)
                    startTime = startTime.AddDays(1);
                //t.EndTime = t.EndTime?.ToLocalTime();

                var newTask = new DeviceScheduleTask()
                {
                    TaskId = t.TaskId,
                    StartTime = startTime,
                    Schedule = s,
                    ScheduleId = s.Id
                };
                allScheduledTasks.Add(newTask);
                if (t.Interval != null)
                {
                    var lengthInMinutes = TimeSpan.FromDays(1).TotalMinutes;

                    if (t.EndTime.HasValue)
                        lengthInMinutes = t.EndTime.Value.TimeOfDay.Subtract(t.StartTime.TimeOfDay).TotalMinutes;

                    var mod = lengthInMinutes % t.Interval;
                    for (var i = 1; i <= ((lengthInMinutes - mod) / t.Interval); i++)
                    {
                        allScheduledTasks.Add(new DeviceScheduleTask()
                        {
                            TaskId = newTask.TaskId,
                            StartTime = newTask.StartTime.AddMinutes(i * t.Interval.Value),
                            Schedule = s,
                            ScheduleId = s.Id
                        });
                    }
                }
            }));
            
            return allScheduledTasks;
        }
        public void Setup(AquariumDevice device)
        {
            _device = device;
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
            switch (task.TaskId)
            {
                case ScheduleTaskTypes.Snapshot:
                    TakeSnapshotTask(task);
                    break;
                case ScheduleTaskTypes.StartATO:
                    PerformATOTask(task);
                    break;
                default:
                    _logger.LogError($"Invalid task type id (taskId: {task.Id})");
                    break;
            }
        }
        private void TakeSnapshotTask(DeviceScheduleTask task)
        {
            _logger.LogInformation("Taking aquarium snapshot...");
            var con = _deviceService.GetConnectionInformation();
            var device = con.Aquarium.Device;
            var cameraConfiguration = device.CameraConfiguration;
            var snapshot = _deviceService.TakeSnapshot();
            var photo = _deviceService.TakePhoto(cameraConfiguration);

            _deviceService.SendAquariumSnapshotToHost(snapshot, photo);
            _logger.LogInformation("Aquarium snapshot sent successfully");
        }
        private void PerformATOTask(DeviceScheduleTask task)
        {
            _atoService.BeginAutoTopOff(new AutoTopOffRequest()
            {
                Runtime = 20
            });
        }
    }
}














