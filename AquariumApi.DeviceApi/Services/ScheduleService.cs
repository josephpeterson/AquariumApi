using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
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
        private readonly IAquariumClient _aquariumClient;

        public IExceptionService _exceptionService { get; }

        private IGpioService _gpioService;
        private readonly IConfiguration _config;
        private Dictionary<ScheduleTaskTypes, DeviceTaskProcess> _deviceTaskCallbacks = new Dictionary<ScheduleTaskTypes, DeviceTaskProcess>();
        private List<RunningScheduledJob> RunningJobs = new List<RunningScheduledJob>();
        private List<ScheduledJob> ScheduledJobsQueue = new List<ScheduledJob>();
        public CancellationToken token;
        public bool Running;


        public ScheduleService(IConfiguration config,
            ILogger<ScheduleService> logger,
            IAquariumAuthService aquariumAuthService,
            IAquariumClient aquariumClient,
            IExceptionService exceptionService,
            IGpioService gpioService)
        {
            _config = config;
            _logger = logger;
            _aquariumAuthService = aquariumAuthService;
            _aquariumClient = aquariumClient;
            _exceptionService = exceptionService;
            _gpioService = gpioService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            token = stoppingToken;
            //stoppingToken.Register(() => Cleanup());

            //_schedules = LoadSchedulesFromCache();
            RunningScheduledJob pendingJob = null;
            GenerateScheduledJobsQueue();
            try
            {
                Running = true;
                while (!stoppingToken.IsCancellationRequested)
                {
                    var scheduledJob = GetNextTask();
                    TopOffScheduledJobsQueue();
                    if (scheduledJob != null)
                    {
                        var task = scheduledJob.Task;
                        var eta = (scheduledJob.StartTime - DateTime.Now);

                        //readable time
                        string time = string.Format("{0:D2}h:{1:D2}m",eta.Hours,eta.Minutes);
                        _logger.LogInformation($"Next task/job scheduled in {time} Maximum Runtime: {scheduledJob.MaximumEndTime} Task: {scheduledJob.Task.Name}");
                        pendingJob = GenericPerformTask(task,eta);
                        ScheduledJobsQueue.Remove(scheduledJob);
                        PrintScheduleStatus();
                        await pendingJob.RunningTask;
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
                if(pendingJob != null)
                {
                    StopScheduledJob(pendingJob.ScheduledJob, JobEndReason.Canceled);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured during schedule: {e.Message}");
                _logger.LogError($"{e.StackTrace}");
            }
            _logger.LogInformation($"Schedule job stopped");
            CleanUp();
        }
        private void GenerateScheduledJobsQueue()
        {
            _logger.LogInformation($"[ScheduleService] Generating scheduled jobs queue...");

            var length = DateTime.Now + TimeSpan.FromDays(7);
            var scheduledJobs = ExpandTasks(DateTime.Now, length);
            _logger.LogInformation($"[ScheduleService] Scheduled jobs queue ready: {scheduledJobs.Count()} scheduled jobs in queue.");
            ScheduledJobsQueue = scheduledJobs;
        }
        private void TopOffScheduledJobsQueue()
        {
            _logger.LogInformation("[ScheduleService] Topping off scheduled jobs queue...");
            if (!ScheduledJobsQueue.Any())
            {
                GenerateScheduledJobsQueue();
                return;
            }

            var finalTaskTime = ScheduledJobsQueue.OrderByDescending(s => s.StartTime).First().StartTime.AddMilliseconds(1);
            var length = DateTime.UtcNow + TimeSpan.FromDays(7);
            length = length.Date + finalTaskTime.TimeOfDay;
            var scheduledJobs = ExpandTasks(finalTaskTime, length);
            _logger.LogInformation($"[ScheduleService] Adding {scheduledJobs.Count()} scheduled jobs to the queue!");
            ScheduledJobsQueue.AddRange(scheduledJobs);
        }
        public List<DeviceSchedule> GetAllSchedules()
        {
            var aq = _aquariumAuthService.GetAquarium();
            if(aq == null || aq.Device == null)
                return new List<DeviceSchedule>();
            var sa = aq.Device.Schedules;
            if (sa == null)
                return new List<DeviceSchedule>();
            return sa.ToList();
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
        public ScheduledJob GetNextTask(DateTime? currentTime = null)
        {
            //return ScheduledJobsQueue.FirstOrDefault();
            return GetAllScheduledTasks()
                .Where(sj => sj.Status == JobStatus.Ready)
                .FirstOrDefault();
        }


        public List<ScheduledJob> GetAllScheduledTasks()
        {
            return RunningJobs.Select(rj => rj.ScheduledJob).Concat(ScheduledJobsQueue)
                .OrderBy(sj => sj.StartTime)
                .ToList();
        }
        public List<ScheduledJob> GetAllRunningJobs()
        {
            return RunningJobs.Select(running => running.ScheduledJob).ToList();
        }
        public void Setup()
        {
            CleanUp();
            var schedules = GetAllSchedules();
            if(schedules.Count() == 0)
            {
                _logger.LogInformation("No schedules are deployed on this device.");
                return;

            }

            _logger.LogInformation($"{schedules.Count()} Schedules found");

            //Register all sensors
            var aq = _aquariumAuthService.GetAquarium();
            aq.Device.Sensors.Where(s => s.Polarity == Polarity.Input).ToList().ForEach(s =>
            {
                _logger.LogInformation($"[ScheduleService] Registering sensor callback for sensor {s.Name} Gpio Pin: {s.Pin}");
                s.OnSensorTriggered = (sender, value) => {
                    OnDeviceSensorTriggered(s);
                };
            });

            StartAsync(new System.Threading.CancellationToken()).Wait();
        }

        /* Tasks */
        public void PerformTask(DeviceScheduleTask task)
        {
            if (_deviceTaskCallbacks.ContainsKey(task.TaskTypeId))
                _deviceTaskCallbacks[task.TaskTypeId](task);
            else
                throw new Exception($"Invalid task type id (taskId: {task.Id})");
        }
        public RunningScheduledJob GenericPerformTask(DeviceScheduleTask task,TimeSpan? initialDelay = null)
        {
            var aq = _aquariumAuthService.GetAquarium();
            var targetSensor = aq.Device.Sensors.First(s => s.Id == task.TargetSensorId);
            DeviceSensor targetSensorValue = aq.Device.Sensors.First(s => s.Id == task.TargetSensorId);
            DeviceSensor triggerSensor = null;
            if (task.TriggerSensorId.HasValue)
            {
                triggerSensor = aq.Device.Sensors.First(s => s.Id == task.TriggerSensorId);
                var triggerSensorValue = _gpioService.GetPinValue(triggerSensor);
                if (triggerSensorValue == task.TriggerSensorValue)
                    throw new Exception($"Trigger sensor is already reading the specified trigger value! Trigger sensor value: {triggerSensorValue}");
            }

            var maxRuntime = TimeSpan.FromSeconds(task.MaximumRuntime.Value);
            CancellationTokenSource cancellationSouce = new CancellationTokenSource();
            CancellationToken ct = cancellationSouce.Token;

            //preflight job
            _logger.LogInformation("[ScheduleService] Beginning new schedule job...");
            var job = new ScheduledJob();
            job.MaximumEndTime = job.StartTime + maxRuntime;
            job.Status = JobStatus.Pending;
            job.Task = task;
            job.TaskId = task.Id.Value;

            if (initialDelay.HasValue)
            {
                job.StartTime = DateTime.Now + initialDelay.Value;
                _ = DispatchJobStatus(job);
            }

            var newRunningTask = Task.Run(() =>
            {
                while (DateTime.Now < job.StartTime)
                {
                    if (ct.IsCancellationRequested)
                    {
                        //_logger.LogInformation("[ScheduleService] Scheduled job completed but canceled...");
                        //StopScheduledJob(job, JobEndReason.Error);
                        return;
                    }
                }
                job.StartTime = DateTime.Now;
                job.Status = JobStatus.Running;
                _ = DispatchJobStatus(job);
                _gpioService.SetPinValue(targetSensor, GpioPinValue.High);
                //now sleep for max run time
                while(DateTime.Now < job.MaximumEndTime.Value)
                {
                    if (ct.IsCancellationRequested)
                    {
                        //_logger.LogInformation("[ScheduleService] Scheduled job completed but canceled...");
                        //StopScheduledJob(job, JobEndReason.Error);
                        return;
                    }
                }
                _logger.LogInformation("[ScheduleService] Scheduled job completed due to maximum run time...");
                StopScheduledJob(job, JobEndReason.MaximumRuntimeReached);
            },cancellationSouce.Token);
            newRunningTask.ConfigureAwait(false);//Fire and forget

            var running = new RunningScheduledJob()
            {
                ScheduledJob = job,
                RunningTask = newRunningTask,
                CancellationSource = cancellationSouce,
            };
            RunningJobs.Add(running);
            return running;
        }

        private void PrintScheduleStatus()
        {
            string str = "";
            string str2 = "";
            RunningJobs.ForEach(j => str += $"\n---- Status: {j.ScheduledJob.Status} Start Time: {j.ScheduledJob.StartTime} Task: {j.ScheduledJob.Task.Name}");
            ScheduledJobsQueue.ForEach(j => str2 += $"\n---- Status: {j.Status} Start Time: {j.StartTime} Task: {j.Task.Name}");
            _logger.LogInformation($"\n------------------------------------\n---- Running Jobs Queue: {RunningJobs.Count()}{str}" +
                $"\n------------------------------------\n---- Scheduled Jobs Queue: {ScheduledJobsQueue.Count()}{str2} \n------------------------------------" +
                $"\n-------------------------------------------------------------------");
        }

        public void CleanUp()
        {
            StopAllScheduledJobs();
            Running = false;
            StopAsync(token).Wait();
        }
        private void OnDeviceSensorTriggered(DeviceSensor sensor)
        {
            //check if any running jobs contain this sensor
            var currentValue = _gpioService.GetPinValue(sensor);
            RunningJobs.Where(j => j.ScheduledJob.Task.TriggerSensorId == sensor.Id && j.ScheduledJob.Task.TriggerSensorValue == currentValue)
                .ToList()
                .ForEach(job =>
                {
                    _logger.LogInformation($"[ScheduleService] Cancelling scheduled job id: {job.ScheduledJob.Id} because {sensor.Name} was triggered to value {currentValue}");
                    job.CancellationSource.Cancel();
                });
        }
        private async Task<ScheduledJob> DispatchJobStatus(ScheduledJob job)
        {
            try
            {
                var s = await _aquariumClient.DispatchScheduledJob(job);
                job.Id = s.Id;
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to dispatch scheduled job status to server");
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                await _exceptionService.Throw(e);
            }
            return job;
        }
        public List<ScheduledJob> ExpandTasks(DateTime startDate,DateTime endDate)
        {
            var length = endDate - startDate;
            var aq = _aquariumAuthService.GetAquarium();
            var allScheduledJobs = new List<ScheduledJob>();

            //get all task assignments that start by the time trigger
            var taskAssignment =  GetAllSchedules().SelectMany(s => s.TaskAssignments)
                .Where(ta => ta.TriggerTypeId == TriggerTypes.Time)
                .ToList();


            for(var k=0;k<length.TotalDays;k++)
                taskAssignment.ForEach(ta =>
            {
                var tod = ta.StartTime.TimeOfDay;
                if (k == 0 && DateTime.Now.TimeOfDay >= tod)
                    return;
                var startTime = startDate.Date + tod;
                //if (startTime < startDate)
                //    startTime = startTime.AddDays(1);
                startTime = startTime.AddDays(k);



                var task = aq.Device.Tasks.First(t => t.Id == ta.TaskId);

                var scheduledJob = new ScheduledJob()
                {
                    TaskId = task.Id.Value,
                    Task = task,
                    StartTime = startTime,
                    MaximumEndTime = startTime + TimeSpan.FromSeconds(task.MaximumRuntime.Value)
                //ScheduleId = Id
            };
                allScheduledJobs.Add(scheduledJob);

                //Check if this task assignment repeats
                if (ta.Repeat)
                {
                    var lengthInMinutes = TimeSpan.FromDays(1).TotalMinutes;

                    lengthInMinutes = ta.RepeatEndTime.TimeOfDay.Subtract(ta.StartTime.TimeOfDay).TotalMinutes;

                    var mod = lengthInMinutes % ta.RepeatInterval;
                    for (var i = 1; i <= ((lengthInMinutes - mod) / ta.RepeatInterval); i++)
                    {
                        allScheduledJobs.Add(new ScheduledJob()
                        {
                            TaskId = scheduledJob.TaskId,
                            Task = scheduledJob.Task,
                            StartTime = scheduledJob.StartTime.AddMinutes(i * ta.RepeatInterval.Value),
                            //ScheduleId = Id
                        });
                    }
                }
            });
            return allScheduledJobs.OrderBy(t => t.StartTime).ToList();
        }
        public class RunningScheduledJob
        {
            public ScheduledJob ScheduledJob { get; set; }
            public Task RunningTask { get; set; }
            public CancellationTokenSource CancellationSource { get; set; }
        }
        public ScheduledJob StopScheduledJob(ScheduledJob scheduledJob,JobEndReason jobEndReason)
        {
            var runningJob = RunningJobs.First(j => j.ScheduledJob.Id == scheduledJob.Id);
            var job = runningJob.ScheduledJob;
            var aq = _aquariumAuthService.GetAquarium();
            var targetSensor = aq.Device.Sensors.First(s => s.Id == job.Task.TargetSensorId);

            runningJob.CancellationSource.Cancel();

            job.EndTime = DateTime.Now;
            job.Status = JobStatus.Completed;
            job.EndReason = jobEndReason;
            job.UpdatedAt = DateTime.Now;
            _gpioService.SetPinValue(targetSensor, GpioPinValue.Low);
            RunningJobs.Remove(runningJob);
            _ = DispatchJobStatus(job);
            _logger.LogInformation($"[ScheduleService] Scheduled job {job.Id} stopped.");
            return job;
        }
        public List<ScheduledJob> StopAllScheduledJobs()
        {
            return RunningJobs.Select(rj => StopScheduledJob(rj.ScheduledJob, JobEndReason.ForceStop)).ToList();
        }









        public delegate void DeviceTaskProcess(DeviceScheduleTask task);
        public void RegisterDeviceTask(ScheduleTaskTypes taskType, DeviceTaskProcess callback)
        {
            _deviceTaskCallbacks[taskType] = callback;
            _logger.LogInformation($"Registered callback for task id: {taskType}");
        }
    }
}














