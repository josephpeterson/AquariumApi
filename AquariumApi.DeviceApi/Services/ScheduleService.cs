using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IScheduleService
    {
        List<DeviceScheduleTask> ExpandSchedule(DeviceSchedule schedule);
        FutureTask GetNextTask(List<DeviceScheduleTask> scheduledTasks, DateTime? currentTime = null);
        List<DeviceSchedule> LoadAllSchedules();
        void SaveScheduleAssignment(List<DeviceSchedule> deviceSchedules);
        void Start();
    }
    public class ScheduleService : IScheduleService
    {
        private IConfiguration _config;
        private ILogger<ScheduleService> _logger;
        private IDeviceService _deviceService;
        private IQueueService _queueService;

        public List<Thread> threads = new List<Thread>();

        public ScheduleService(IConfiguration config, ILogger<ScheduleService> logger, IDeviceService deviceService, IQueueService queueService)
        {
            _config = config;
            _logger = logger;
            _deviceService = deviceService;
            _queueService = queueService;
        }

        public void Start()
        {
            Stop();

            var deviceSchedules = LoadAllSchedules();
            _logger.LogWarning($"{deviceSchedules.Count} schedules have been loaded");


            deviceSchedules.ForEach(schedule =>
            {
                try
                {
                    BeginSchedule(schedule);
                }
                catch (Exception e)
                {
                    _logger.LogWarning($"Could not start schedule (Schedule: {schedule.Name}) Exception: {e.Message}");
                }
            });
            _logger.LogWarning($"{threads.Count} schedules have started");
        }
        public void Stop()
        {
            _logger.LogWarning($"Stopping schedule with {threads.Count} threads...");
            threads.Where(t => !t.IsAlive).ToList().ForEach(t =>
            {
                t.Abort();
            });
        }

        public void BeginSchedule(DeviceSchedule schedule)
        {
            var thread = new ScheduleThread(schedule, this, _deviceService, _queueService);
            var thr = new Thread(new ThreadStart(thread.Main));
            thr.Start();
            threads.Add(thr);
        }




        public void SaveScheduleAssignment(List<DeviceSchedule> deviceSchedules)
        {
            var filepath = _config["ScheduleAssignmentPath"];
            var json = JsonConvert.SerializeObject(deviceSchedules);
            System.IO.File.WriteAllText(filepath, json);
            _logger.LogWarning($"Schedule assignments saved ({deviceSchedules.Count} schedules written)");
        }
        public List<DeviceSchedule> LoadAllSchedules()
        {
            var filepath = _config["ScheduleAssignmentPath"];
            var json = System.IO.File.ReadAllText(filepath);
            return JsonConvert.DeserializeObject<List<DeviceSchedule>>(json);
        }


        //Maybe move this to the model
        public List<DeviceScheduleTask> ExpandSchedule(DeviceSchedule schedule)
        {
            var tasks = new List<DeviceScheduleTask>();
            var indvidualTasks = schedule.Tasks.ToList();
            indvidualTasks.ForEach(t =>
            {
                tasks.Add(new DeviceScheduleTask()
                {
                    TaskId = t.TaskId,
                    StartTime = t.StartTime,
                    ScheduleId = t.ScheduleId,
                    Schedule = t.Schedule
                });
                if (t.Interval != null)
                {
                    var endTime = t.EndTime;
                    if (endTime < t.StartTime)
                        endTime = t.StartTime.AddDays(1);
                    TimeSpan length = endTime.Subtract(t.StartTime);
                    var lengthInMinutes = length.TotalMinutes;
                    var mod = lengthInMinutes % t.Interval;

                    for (var i = 1; i < (lengthInMinutes - mod) / t.Interval; i++)
                    {
                        tasks.Add(new DeviceScheduleTask()
                        {
                            TaskId = t.TaskId,
                            StartTime = t.StartTime.AddMinutes(i * t.Interval.Value),
                            ScheduleId = t.ScheduleId,
                            Schedule = t.Schedule
                        });
                    }
                }
            });
            return tasks.OrderBy(t => t.StartTime).ToList();
        }

        public FutureTask GetNextTask(List<DeviceScheduleTask> scheduledTasks, DateTime? currentTime = null)
        {
            var now = DateTime.Now.TimeOfDay;
            if (currentTime != null)
                now = currentTime.Value.TimeOfDay;

            var futureTasks = scheduledTasks.Where(task =>
            {
                var taskTime = task.StartTime.TimeOfDay;
                return taskTime > now;
            });

            var nextTask = futureTasks.First();
            return new FutureTask
            {
                task = nextTask,
                eta = nextTask.StartTime.TimeOfDay.Subtract(now)
            };
        }


        public void ThreadLoop()
        {

        }
}

    public class ScheduleThread
    {
        public IConfiguration _config;
        private IDeviceService _deviceService;
        private IScheduleService _scheduleService;
        private IQueueService _queueService;

        private List<DeviceScheduleTask> scheduledTasks;
        private DeviceSchedule _schedule;

        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public ScheduleThread(DeviceSchedule schedule)
        {
            _schedule = schedule;

            RegisterServices();

            _logger.Info("Expanding schedule...");
            scheduledTasks = _scheduleService.ExpandSchedule(schedule);
        }

        public void RegisterServices()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"config.json", optional: false)
                .AddEnvironmentVariables();
            _config = builder.Build();


            // 1. Create a new Simple Injector container
            var container = new Container();

            // 2. Configure the container (register)
            //container.Register<IConfiguration, null>();
            container.Register<NLog.ILogger, Logger>(Lifestyle.Singleton);
            container.Register<IHardwareService, HardwareService>();
            container.Register<ISerialService, SerialService>();
            container.Register<IAquariumClient, AquariumClient>();
            container.Register<IDeviceService, DeviceService>();
            container.Register<IScheduleService, ScheduleService>();
            container.Register<IQueueService, QueueService>();

            // 3. Optionally verify the container's configuration.
            container.Verify();
        }
        public void Main()
        {
            var ticks = 0;
            while (true)
            {
                ticks++;
                var task = _scheduleService.GetNextTask(scheduledTasks);
                _logger.Info($"Next task scheduled in {task.eta.TotalMinutes} seconds (Schedule: {task.task.Schedule.Name})");
                Thread.Sleep(task.eta);

                try
                {
                    _logger.Info($"Performing task (TaskId: {task.task.TaskId} Schedule: {task.task.Schedule.Name}");
                    PerformTask(task.task);
                }
                catch (Exception e)
                {
                    _logger.Error($"Could not perform task [taskId:{task.task.TaskId} Schedule: {task.task.Schedule.Name}: Error: {e.Message}");
                }
            }
        }
        public void PerformTask(DeviceScheduleTask task)
        {
            switch (task.Id)
            {
                case 1:
                    TakeSnapshotTask(task);
                    break;
                default:
                    _logger.Error($"Invalid task type id '{task.Id}");
                    break;
            }
        }

        private void TakeSnapshotTask(DeviceScheduleTask task)
        {
            _logger.Info("Taking snapshot...");
            var device = _deviceService.GetDevice();
            try
            {
                var snapshot = _deviceService.TakeSnapshot();
                var photo = _deviceService.TakePhoto(device.CameraConfiguration);

                try
                {
                    var deviceId = task.Schedule.ScheduleAssignments
                        .Where(sa => sa.ScheduleId == task.ScheduleId)
                        .Select(sa => sa.DeviceId)
                        .First();
                    _deviceService.SendAquariumSnapshotToHost(task.Schedule.Host, deviceId, snapshot, photo);
                }
                catch (Exception e)
                {
                    //Queue snapshot todo wont work currently
                    _queueService.QueueAquariumSnapshot(snapshot, photo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"TakeSnapshot: { ex.Message } Details: { ex.ToString() }");
            }
        }

    }

    public class FutureTask
    {
        public TimeSpan eta;
        public DeviceScheduleTask task;
    }
}
