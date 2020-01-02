using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        public ScheduleService(IConfiguration config, ILogger<ScheduleService> logger,IDeviceService deviceService,IQueueService queueService)
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
                catch(Exception e)
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
            var thread = new ScheduleThread(schedule, this, _logger, _deviceService, _queueService);
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
                if(t.Interval != null)
                {
                    var endTime = t.EndTime;
                    if (endTime < t.StartTime)
                        endTime = t.StartTime.AddDays(1);
                    TimeSpan length = endTime.Subtract(t.StartTime);
                    var lengthInMinutes = length.TotalMinutes;
                    var mod = lengthInMinutes % t.Interval;

                    for(var i=1;i<(lengthInMinutes - mod)/t.Interval;i++)
                    {
                        tasks.Add(new DeviceScheduleTask()
                        {
                            TaskId = t.TaskId,
                            StartTime = t.StartTime.AddMinutes(i*t.Interval.Value),
                            ScheduleId = t.ScheduleId,
                            Schedule = t.Schedule
                        });
                    }
                }
            });
            return tasks.OrderBy(t => t.StartTime).ToList();
        }

        public FutureTask GetNextTask(List<DeviceScheduleTask> scheduledTasks,DateTime? currentTime = null)
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

        }

    public class ScheduleThread
    {
        private IConfiguration _config;
        private ILogger<ScheduleService> _logger;
        private IDeviceService _deviceService;
        private IScheduleService _scheduleService;
        private IQueueService _queueService;

        private List<DeviceScheduleTask> scheduledTasks;
        private DeviceSchedule _schedule;

        public ScheduleThread(DeviceSchedule schedule,IScheduleService scheduleService, ILogger<ScheduleService> logger, IDeviceService deviceService, IQueueService queueService)
        {
            _logger = logger;
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _queueService = queueService;
            _queueService = queueService;
            _schedule = schedule;

            _logger.LogWarning("Expanding schedule...");
            scheduledTasks = _scheduleService.ExpandSchedule(schedule);
        }
        public void Main()
        {
            var ticks = 0;
            while (true)
            {
                ticks++;
                var task = _scheduleService.GetNextTask(scheduledTasks);
                _logger.LogWarning($"Next task scheduled in {task.eta.TotalMinutes} seconds (Schedule: {task.task.Schedule.Name})");
                Thread.Sleep(task.eta);

                try
                {
                    _logger.LogWarning($"Performing task (TaskId: {task.task.TaskId} Schedule: {task.task.Schedule.Name}");
                    PerformTask(task.task);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Could not perform task [taskId:{task.task.TaskId} Schedule: {task.task.Schedule.Name}: Error: {e.Message}");
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
                    _logger.LogError($"Invalid task type id '{task.Id}");
                    break;
            }
        }

        private void TakeSnapshotTask(DeviceScheduleTask task)
        {
            _logger.LogWarning("Taking snapshot...");
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
                _logger.LogError($"TakeSnapshot: { ex.Message } Details: { ex.ToString() }");
            }
        }

    }

    public class FutureTask
    {
        public TimeSpan eta;
        public DeviceScheduleTask task;
    }
}
