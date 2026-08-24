using AquariumApi.DeviceApi.Clients;
using AquariumApi.DeviceApi.Providers;
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
using static System.Collections.Specialized.BitVector32;

namespace AquariumApi.DeviceApi
{
    public class ScheduleService : BackgroundService, IDeviceSetupService
    {
        private readonly ILogger<ScheduleService> _logger;
        private readonly DateTimeProvider _dateTimeProvider;
        private IAquariumAuthService _aquariumAuthService;
        private IWirelessDeviceService _wirelessDeviceService;
        private readonly IAquariumClient _aquariumClient;

        public IExceptionService _exceptionService { get; }

        private IGpioService _gpioService;
        private IDeviceConfigurationService _deviceConfiguration;
        private readonly IConfiguration _config;
        private Dictionary<ScheduleTaskTypes, DeviceTaskProcess> _deviceTaskCallbacks = new Dictionary<ScheduleTaskTypes, DeviceTaskProcess>();
        private ScheduleState ScheduleState = new ScheduleState();

        public CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        public bool Running;


        public ScheduleService(IConfiguration config,
            ILogger<ScheduleService> logger,
            DateTimeProvider dateTimeProvider,
            IAquariumAuthService aquariumAuthService,
            IWirelessDeviceService wirelessDeviceService,
            IAquariumClient aquariumClient,
            IExceptionService exceptionService,
            IDeviceConfigurationService deviceConfigurationService,
            IGpioService gpioService)
        {
            _config = config;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _aquariumAuthService = aquariumAuthService;
            _wirelessDeviceService = wirelessDeviceService;
            _aquariumClient = aquariumClient;
            _exceptionService = exceptionService;
            _gpioService = gpioService;
            _deviceConfiguration = deviceConfigurationService;
        }
        public void Setup()
        {
            CleanUp();
            
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            var schedules = deviceConfiguration.Schedules;
            var tasks = deviceConfiguration.Tasks;
            var allSensors = deviceConfiguration.Sensors;
            var gpioSensors = allSensors.Where(x => !x.LocationId.HasValue).ToList();
            var wirelessSensors = allSensors.Where(x => x.LocationId.HasValue).ToList();
            _logger.LogInformation($"Sensor Information:\r\n\t{gpioSensors.Count()} onboard sensors found\r\n\t{wirelessSensors.Count()} wireless sensors found");
            _gpioService.Setup(gpioSensors);

            //Apply wireless sensors
            var sensorsToSetup = wirelessSensors;
            if(sensorsToSetup.Any())
            {
                _logger.LogInformation($"Setting up {sensorsToSetup.Count()} wireless sensors.");
                ResetDeviceSensors(sensorsToSetup);
            }



            _logger.LogInformation($"Schedule Information:\r\n- {schedules.Count()} Schedules found\r\n- {tasks.Count()} tasks found");
            //Register all sensors
            var listeningSensors = gpioSensors.Where(s => s.Polarity == Polarity.Input && !s.LocationId.HasValue).ToList();
            _logger.LogInformation($"{listeningSensors.Count()} readable sensors found. Registering input triggers...");
            listeningSensors.ForEach(s =>
            {
                _logger.LogInformation($"[ScheduleService] Registering sensor callback for sensor {s.Name} Gpio Pin: {s.Pin}");
                s.OnSensorTriggered = (sender, value) => {
                    _logger.LogWarning($"Device Sensor {s.Name} triggered to {value}");
                    OnDeviceSensorTriggered(s);
                };
            });
            if (schedules.Count() == 0)
            {
                _logger.LogInformation("No schedules are deployed on this device. Will not begin ScheduleService loop");
                return;
            }

            _cancellationSource = new CancellationTokenSource();
            StartAsync(_cancellationSource.Token);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
                ScheduleState.Running = true;
                GenerateScheduledJobsQueue();
                ScheduleState.NextTasks = GetNextTask();
                await Task.Run(() =>
                {

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        if (!ScheduleState.NextTasks.Any())
                        {
                            _logger.LogWarning($"No next tasks discovered, ending schedule.");
                            break;
                        }
                        var nextTaskTime = ScheduleState.NextTasks.First().StartTime;
                        var eta = (nextTaskTime - _dateTimeProvider.Now);
                            string time = string.Format("{0:D2}h:{1:D2}m", eta.Hours, eta.Minutes);
                        string str = $"\r\n\tNext task/job scheduled in {time} at {nextTaskTime}";
                        string str2 = "";
                        ScheduleState.RunningJobs.ForEach(j => str += $"\n\tStatus:\t{j.ScheduledJob.Status}\tStart Time:\t{j.ScheduledJob.StartTime.Date.ToShortDateString()} {j.ScheduledJob.StartTime.TimeOfDay.ToString("g")}\tTask: {deviceConfiguration.Tasks.First(t => t.Id == j.ScheduledJob.TaskId).Name}");
                        ScheduleState.Scheduled.ForEach(j => str2 += $"\n\tStatus:\t{j.Status}\tStart Time:\t{j.StartTime.Date.ToShortDateString()} {j.StartTime.TimeOfDay.ToString("g")}\tTask: {deviceConfiguration.Tasks.First(t => t.Id == j.TaskId).Name}");
                            

                        ScheduleState.NextTasks.Select(t => deviceConfiguration.Tasks.First(tt => tt.Id == t.TaskId))
                        .ToList()
                        .ForEach(task =>
                        {
                            str += $"\r\n\tTask: {task.Name} Actions: {task.Actions.Count()} Maximum Runtime: {task.MaximumRuntime}";
                        });

                        _logger.LogInformation($"\n------------------------------------\n\tRunning Jobs Queue: {ScheduleState.RunningJobs.Count()}{str}" +
                            $"\n------------------------------------\n\tScheduled Jobs Queue: {ScheduleState.Scheduled.Count()}{str2}" +
                            $"\n-------------------------------------------------------------------");

                        if (eta.TotalSeconds > 30)
                            Thread.Sleep(eta);

                        ScheduleState.NextTasks.ForEach(scheduledJob => 
                        {
                            try
                            {
                                GenericPerformTask(scheduledJob);
                            }
                            catch(DeviceException ex)
                            {
                                _logger.LogWarning($"GenericPerformTask threw a DeviceException: {ex}");
                                ArchiveScheduledJob(scheduledJob);
                            }
                            catch(Exception ex)
                            {
                                _logger.LogError($"Error occured during task: {ex.Message}");
                                _logger.LogError($"{ex.StackTrace}");
                                ArchiveScheduledJob(scheduledJob);
                            }
                            ScheduleState.Scheduled.Remove(scheduledJob);
                        });
                        GenerateScheduledJobsQueue();
                        ScheduleState.NextTasks = GetNextTask();
                    }
                }, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured during schedule: {e.Message}");
                _logger.LogError($"{e.StackTrace}");
            }
            _logger.LogInformation($"Schedule service stopped.");
            CleanUp();
        }
        private void GenerateScheduledJobsQueue()
        {
            ScheduleState.Scheduled.Clear();
            _logger.LogInformation($"Generating scheduled jobs queue...");
            var schedules = _deviceConfiguration.LoadDeviceConfiguration()?.Schedules;
            var length = _dateTimeProvider.Now + TimeSpan.FromDays(7);
            var scheduledJobs = ExpandTasks(_dateTimeProvider.Now, length);
            _logger.LogInformation($"Queue properties:\n-- Timespan: {length}\n-- Schedules: {GetAllSchedules().Count()}\n-- Scheduled Jobs in queue: {scheduledJobs.Count()}");
            _logger.LogInformation($"Scheduled jobs queue ready: {scheduledJobs.Count()} scheduled jobs in queue.");
            ScheduleState.Scheduled = scheduledJobs;
        }
        public List<ScheduledJob> GetNextTask()
        {
            var scheduledJobs = ScheduleState.Scheduled
                .Where(x => x.StartTime - _dateTimeProvider.Now > TimeSpan.FromSeconds(30))
                .OrderBy(x => x.StartTime);
            if (!scheduledJobs.Any())
                return scheduledJobs.ToList();
            var nextTasks = ScheduleState.Scheduled.Where(x => x.StartTime == scheduledJobs.First().StartTime);
            return nextTasks.ToList();
        }
        /* Tasks */
        public RunningScheduledJob GenericPerformTask(ScheduledJob job)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            var sensors = deviceConfiguration.Sensors;
            var tasks = deviceConfiguration.Tasks;
            var task = tasks.First(t => t.Id == job.TaskId);
            var aq = _aquariumAuthService.GetAquarium();


            //Check trigger sensor
            if (task.TriggerSensorId.HasValue)
            {
                var triggerSensor = sensors.First(s => s.Id == task.TriggerSensorId);
                var triggerSensorValue = _gpioService.GetPinValue(triggerSensor);
                if (triggerSensorValue == task.TriggerSensorValue)
                {
                    _logger.LogInformation($"Trigger sensor is already reading the specified trigger value! Trigger sensor value: {triggerSensorValue}");
                    throw new DeviceException($"Trigger sensor is already reading the specified trigger value! Trigger sensor value: {triggerSensorValue}");
                }
            }

            //Check if any sensors are busy
            var busySensorIds = tasks.Where(t => ScheduleState.RunningJobs.Select(r => r.ScheduledJob.TaskId).Any(v => v == t.Id))
                .SelectMany(t => t.Actions)
                .Select(a => a.TargetSensorId);
            if(busySensorIds.Any(s => task.Actions.Any(a => a.TargetSensorId == s)))
                throw new DeviceException($"Task target sensors are already in use by a running job.");



            var maxRuntime = TimeSpan.FromSeconds(task.MaximumRuntime); //maybe switch this to job.MaximumEndTime
            _logger.LogInformation($"[ScheduleService] Beginning new schedule job for maximum runtime: {maxRuntime}");


            CancellationTokenSource cancellationSouce = new CancellationTokenSource();
            CancellationToken ct = cancellationSouce.Token;
            job.StartTime = _dateTimeProvider.Now;
            job.MaximumEndTime = job.StartTime + maxRuntime;
            job.Status = JobStatus.Pending;
            job.TaskId = task.Id.Value;
            job.UpdatedAt = _dateTimeProvider.Now;
            job.Status = JobStatus.Running;
            job.UpdatedAt = _dateTimeProvider.Now;
            //_ = DispatchJobStatus(job);
            _logger.LogInformation($"[ScheduleService] Task end time: {job.MaximumEndTime}");


            List<DeviceSensor> openedSensors = new List<DeviceSensor>();


            //Trigger a groupby update for all wireless devices
            var wirelessDeviceActions = task.Actions.Where(a => sensors.First(s => s.Id == a.TargetSensorId).LocationId.HasValue).ToList();
            var wirelessDeviceStatuses = wirelessDeviceActions.GroupBy(p => sensors.First(s => s.Id == p.TargetSensorId).LocationId,
            (key, g) =>
            {
                var wd = deviceConfiguration.WirelessDevices.First(wd => wd.Id == key);
                try
                {
                    var wdSensors = new List<DeviceSensor>();
                    var sensorActions = g.Select(action =>
                    {
                        var sensor = sensors.First(s => s.Id == action.TargetSensorId);
                        wdSensors.Add(sensor);
                        return new WirelessDeviceSensor()
                        {
                            Pin = sensor.Pin,
                            Value = action.TargetSensorValue,
                            Length = maxRuntime.TotalMilliseconds,
                            InitialValue = sensor.AlwaysOn ? GpioPinValue.High : GpioPinValue.Low,
                        };
                    }).ToList();
                    _logger.LogInformation($"Triggering sensors on wireless device {wd.Name} Sensor count: {sensorActions.Count()}");
                    var status = _wirelessDeviceService.TriggerWirelessDeviceSensor(wd, sensorActions).Result;
                    openedSensors.AddRange(wdSensors);
                    return status;
                }
                catch (Exception e)
                {
                    ResetDeviceSensors(openedSensors);
                    _logger.LogError(e.Message);
                    throw new DeviceException($"Error occurred while communicating with wireless device {wd.Name} ({wd.Hostname}).");
                }
            }).ToList();


            var gpioActions = task.Actions.Where(a => !sensors.First(s => s.Id == a.TargetSensorId).LocationId.HasValue).ToList();
            //Complete GPIO Actions
            try
            {
                gpioActions.ForEach(a =>
                {
                    var sensor = sensors.First(x => x.Id == a.TargetSensorId);
                    openedSensors.Add(sensor);
                    _logger.LogInformation($"Triggering onboard sensor {sensor.Name} to Value: {GpioPinValue.Low} Location: {sensor.LocationId}");
                    if (!sensor.LocationId.HasValue)
                        _gpioService.SetPinValue(sensor, a.TargetSensorValue);
                });
            }
            catch(Exception e)
            {
                ResetDeviceSensors(openedSensors);
                throw new DeviceException("Error occurred while setting GPIO values.");
            }




            var newRunningTask = Task.Run(() =>
            {
                //now sleep for max run time
                while(_dateTimeProvider.Now < job.MaximumEndTime.Value)
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
            ScheduleState.RunningJobs.Add(running);
            return running;
        }
        public RunningScheduledJob GenericPerformTask(DeviceScheduleTask task)
        {
            var r = new Random();
            int? v = null;
            while (v == null)
                v = r.Next(1000, 999999);

            return GenericPerformTask(new ScheduledJob()
            {
                Id = v.Value,
                TaskId = task.Id.Value,
                StartTime = _dateTimeProvider.Now,
                MaximumEndTime = _dateTimeProvider.Now.AddSeconds(task.MaximumRuntime)
            });
        }

        public void CleanUp()
        {
            ScheduleState.RunningJobs.Select(rj => StopScheduledJob(rj.ScheduledJob, JobEndReason.ForceStop)).ToList();
            ScheduleState.Scheduled.Clear();
            Running = false;
            if(!_cancellationSource.IsCancellationRequested)
            { 
                _cancellationSource.Cancel();
                StopAsync(_cancellationSource.Token).Wait();
            }
        }
        private void OnDeviceSensorTriggered(DeviceSensor sensor)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            //check if any running jobs contain this sensor
            var currentValue = _gpioService.GetPinValue(sensor);
            //_logger.LogInformation($"[ScheduleService] OnDeviceSensorTriggered: Device Sensor triggered {sensor.Name} (ID: {sensor.Id}) was triggered to value {currentValue}");
            ScheduleState.RunningJobs.Where(j => {
                var task = deviceConfiguration.Tasks.First(t => t.Id == j.ScheduledJob.TaskId);
                return task.TriggerSensorId == sensor.Id;
            })
                .ToList()
                .ForEach(job =>
                {
                    _logger.LogInformation($"[ScheduleService] Cancelling scheduled job id: {job.ScheduledJob.Id} because {sensor.Name} was triggered to value {currentValue}");
                    StopScheduledJob(job.ScheduledJob, JobEndReason.Normally);
                });
        }
        #region Schedule State
        public ScheduleState GetScheduleStatus()
        {
            return ScheduleState;
        }

        public ScheduledJob StopScheduledJob(ScheduledJob scheduledJob, JobEndReason jobEndReason)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            var sensors = deviceConfiguration.Sensors;
            var task = deviceConfiguration.Tasks.First(t => t.Id == scheduledJob.TaskId);
            var runningJob = ScheduleState.RunningJobs.First(j => j.ScheduledJob.Id == scheduledJob.Id);
            var job = runningJob.ScheduledJob;
            var aq = _aquariumAuthService.GetAquarium();

            runningJob.CancellationSource.Cancel();

            job.EndTime = _dateTimeProvider.Now;
            job.Status = JobStatus.Completed;
            job.EndReason = jobEndReason;
            job.UpdatedAt = _dateTimeProvider.Now;

            var openedSensors = sensors.Where(s => task.Actions.Any(a => a.TargetSensorId == s.Id)).ToList();
            ResetDeviceSensors(openedSensors);

            ScheduleState.RunningJobs.Remove(runningJob);
            //_ = DispatchJobStatus(job);
            _logger.LogInformation($"[ScheduleService] Scheduled job {job.Id} stopped.");

            if (jobEndReason == JobEndReason.Normally || jobEndReason == JobEndReason.MaximumRuntimeReached) //maybe move this out to where this method gets called
            {
                //find continuing task
                /*
                var taskAssignments = GetAllSchedules().SelectMany(s => s.Tasks)
                    .Where(ta => ta.TriggerTypeId == TriggerTypes.TaskDependent && ta.TriggerTaskId == job.TaskId);
                if (taskAssignments.Any())
                {
                    _logger.LogInformation($"[ScheduleService] Scheduled job {job.Id} will continue with another {taskAssignments.Count()} task(s)");
                    taskAssignments.ToList().ForEach(ta =>
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                GenericPerformTask(new ScheduledJob()
                                {
                                    TaskId = ta.TaskId,
                                    PreviousJob = job
                                });
                            }
                            catch(DeviceException ex)
                            {
                                //we probably did not need to run this
                            }
                        });
                        
                    });

                }
                */
            }
            ArchiveScheduledJob(job);
            return job;
        }
        private void ResetDeviceSensors(List<DeviceSensor> deviceSensors = null)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            if (deviceSensors == null)
                deviceSensors = deviceConfiguration.Sensors;
            var resetSensors = new List<DeviceSensor>();
            deviceConfiguration.WirelessDevices.ForEach(wd =>
            {
                var wdSensors = deviceSensors.Where(s => s.LocationId == wd.Id)
                .Select(s => new WirelessDeviceSensor()
                {
                    Pin = s.Pin,
                    Value = s.AlwaysOn ? GpioPinValue.High : GpioPinValue.Low,
                    InitialValue = s.AlwaysOn ? GpioPinValue.High : GpioPinValue.Low,
                    Length = -1
                })
                .ToList();
                if (!wdSensors.Any())
                    return;

                try
                {
                    _logger.LogInformation($"Resetting device sensors on wireless device {wd.Name} Sensor count: {wdSensors.Count()}");
                    _wirelessDeviceService.TriggerWirelessDeviceSensor(wd, wdSensors).Wait();
                    resetSensors.AddRange(deviceSensors.Where(s => s.LocationId == wd.Id));
                }
                catch(Exception e)
                {
                    _logger.LogWarning($"Could not reset sensors on wireless device: Id: {wd.Id} Hostname: {wd.Hostname}");
                }
            });
            deviceSensors.Where(s => !s.LocationId.HasValue).ToList().ForEach(sensor =>
            {
                try
                {
                    var val = sensor.AlwaysOn ? GpioPinValue.High : GpioPinValue.Low;
                    _logger.LogInformation($"Resetting onboard device sensor {sensor.Name} to Value: {val}");
                    _gpioService.SetPinValue(sensor, val);
                    resetSensors.Add(sensor);
                }
                catch (Exception e)
                {
                    _logger.LogWarning($"Could not reset onboard sensor {sensor.Name} Pin: {sensor.Pin}");
                }
            });
            _logger.LogInformation($"Reset {resetSensors.Count()} sensor(s).");
        }

        #endregion
        public delegate void DeviceTaskProcess(DeviceScheduleTask task);
        public void RegisterDeviceTask(ScheduleTaskTypes taskType, DeviceTaskProcess callback)
        {
            _deviceTaskCallbacks[taskType] = callback;
            _logger.LogInformation($"Registered callback for task id: {taskType}");
        }
        #region Schedule CRUD
        public List<DeviceSchedule> GetAllSchedules()
        {
            var schedules = _deviceConfiguration.LoadDeviceConfiguration()?.Schedules;
            return schedules;
        }
        public List<DeviceSchedule> UpsertDeviceSchedule(DeviceSchedule deviceSchedule)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            if (!deviceSchedule.Id.HasValue)
            {
                var r = new Random();
                int? v = null;
                while (v == null || deviceConfiguration.Schedules.Any(tt => tt.Id == v))
                    v = r.Next(1000, 9999);
                deviceSchedule.Id = v;
            }


            //Validate
            if (string.IsNullOrEmpty(deviceSchedule.Name))
                throw new DeviceException($"Schedule must contain a valid name.");
            if (!deviceSchedule.Tasks.Any(t => deviceConfiguration.Tasks.Any(s => s.Id == t.Id)))
                throw new DeviceException($"Schedules must contain valid tasks.");
            if (!deviceSchedule.Tasks.Any())
                throw new DeviceException($"Schedules must at least one task.");
            if (string.IsNullOrEmpty(deviceSchedule.DateConditions) 
                || deviceSchedule.DateConditions.Length != 7 
                || deviceSchedule.DateConditions.Replace("0","").Replace("1", "").Length > 0)
                throw new DeviceException($"Schedule date conditions are invalid.");

            var schedules = deviceConfiguration.Schedules.Where(t => t.Id != deviceSchedule.Id && t.Id.HasValue).ToList();
            schedules.Add(deviceSchedule);
            deviceConfiguration.Schedules = schedules.OrderBy(s => s.Name).ToList();

            _logger.LogInformation($"Saving {schedules.Count()} to device...");
            _deviceConfiguration.SaveDeviceConfiguration(deviceConfiguration);

            Setup();
            return schedules;
        }
        public List<DeviceSchedule> RemoveDeviceSchedule(DeviceSchedule deviceSchedule)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            deviceConfiguration.Schedules = deviceConfiguration.Schedules.Where(t => t.Id != deviceSchedule.Id).ToList();
            _logger.LogInformation($"Saving {deviceConfiguration.Schedules.Count()} tasks to device...");
            _deviceConfiguration.SaveDeviceConfiguration(deviceConfiguration);
            Setup();
            return deviceConfiguration.Schedules;
        }
        #endregion
        #region Task CRUD
        public List<DeviceScheduleTask> GetAllTasks()
        {
            var tasks = _deviceConfiguration.LoadDeviceConfiguration()?.Tasks;
            return tasks;
        }
        public List<DeviceScheduleTask> UpsertDeviceTask(DeviceScheduleTask deviceTask)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            if (!deviceTask.Id.HasValue)
            {
                var r = new Random();
                int? v = null;
                while (v == null || deviceConfiguration.Tasks.Any(tt => tt.Id == v))
                    v = r.Next(1000, 9999);
                deviceTask.Id = v;
            }

            //Validate
            if (string.IsNullOrEmpty(deviceTask.Name))
                throw new DeviceException($"Tasks must contain a name.");
            if (deviceTask.MaximumRuntime <= 0)
                throw new DeviceException($"Task must have a maximum runtime greater than 0.");
            if(deviceTask.Actions.Select(a => a.TargetSensorId).Any(targetSensorId => !deviceConfiguration.Sensors.Any(s => s.Id == targetSensorId)))
                throw new DeviceException($"Task actions must contain a valid target sensor.");
            if (deviceTask.TriggerSensorId != null && !deviceConfiguration.Sensors.Any(s => s.Id == deviceTask.TriggerSensorId))
                throw new DeviceException($"Tasks must contain a valid trigger sensor.");
            if (deviceTask.TriggerSensorId != null && deviceTask.TriggerSensorValue == null)
                throw new DeviceException($"Tasks must contain a valid trigger sensor and value combination.");
            if (!deviceTask.Actions.Any())
                throw new DeviceException($"Tasks must contain at least one action.");

            var tasks = deviceConfiguration.Tasks.Where(t => t.Id != deviceTask.Id && t.Id.HasValue).ToList();
            tasks.Add(deviceTask);
            deviceConfiguration.Tasks = tasks.OrderBy(s => s.Name).ToList();
            _logger.LogInformation($"Saving {tasks.Count()} tasks to device...");
            _deviceConfiguration.SaveDeviceConfiguration(deviceConfiguration);
            Setup();
            return tasks;
        }
        public List<DeviceScheduleTask> RemoveDeviceTask(DeviceScheduleTask deviceTask)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();

            //Validate
            if (GetAllSchedules().SelectMany(s => s.Tasks).Any(t => t.Id == deviceTask.Id))
                throw new DeviceException("Device sensor is being used within a schedule.");

            deviceConfiguration.Tasks = deviceConfiguration.Tasks.Where(t => t.Id != deviceTask.Id).ToList();
            _logger.LogInformation($"Saving {deviceConfiguration.Tasks.Count()} tasks to device...");
            _deviceConfiguration.SaveDeviceConfiguration(deviceConfiguration);
            Setup();
            return deviceConfiguration.Tasks;
        }
        #endregion
        #region Device Sensor CRUD
        public List<DeviceSensor> GetDeviceSensorValues(bool onboardOnly = false)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            var sensors = deviceConfiguration.Sensors;
            var onboardSensors = _gpioService.GetAllSensors();
            onboardSensors.ForEach(s => sensors.First(ss => ss.Id == s.Id).Value = s.Value);

            if (onboardOnly)
                return sensors;

            //Get wireless sensor values
            var wirelessDeviceStatuses = _wirelessDeviceService.GetWirelessDeviceStatuses().Result;
            
            sensors.Where(s => s.LocationId.HasValue).ToList().ForEach(s =>
            {
                var wirelessDevice = deviceConfiguration.WirelessDevices.FirstOrDefault(wd => wd.Id == s.LocationId);
                var wirelessDeviceStatus = wirelessDeviceStatuses.FirstOrDefault(s => s.Hostname == wirelessDevice.Hostname);
                if(wirelessDeviceStatus != null)
                    s.Value = wirelessDeviceStatus.Sensors.First(v => v.Pin == s.Pin).Value;
            });

            sensors.Where(s => !s.Value.HasValue).ToList().ForEach(s =>
            {
                _logger.LogWarning($"DeviceSensor {s.Name} (Pin: {s.Pin})'s value was not found onboard or wirelessly.");
            });

            return sensors;
        }

        public DeviceSensorTestRequest TestDeviceSensor(DeviceSensorTestRequest testRequest)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            var sensor = deviceConfiguration.Sensors.Where(s => s.Id == testRequest.SensorId).FirstOrDefault();
            if (sensor == null) throw new DeviceException($"Could not locate sensor on this device by that sensor id ({testRequest.SensorId}).");

            var maxTestRuntime = Convert.ToInt32(deviceConfiguration.Settings.DeviceSensorTestMaximumRuntime);
            if (testRequest.Runtime > maxTestRuntime) throw new DeviceException($"Runtime exceeds maximum runtime allowed (Maximum allowed: {maxTestRuntime})");
            maxTestRuntime = testRequest.Runtime;


            var sensorValue = GpioPinValue.High;
            var endValue = GpioPinValue.Low;
            if (sensor.AlwaysOn) //invert the value we are setting
            {
                sensorValue = GpioPinValue.Low;
                endValue = GpioPinValue.High;
            }

            if (!sensor.LocationId.HasValue)
            {
                _logger.LogWarning($"Testing onboard sensor ({sensor.Name} Pin Number: {sensor.Pin} Polarity: {sensor.Polarity})");
                testRequest.StartTime = DateTime.UtcNow;
                _gpioService.SetPinValue(sensor, sensorValue);
                Thread.Sleep(TimeSpan.FromSeconds(maxTestRuntime));
                _gpioService.SetPinValue(sensor, endValue);
                testRequest.EndTime = DateTime.UtcNow;
            }
            else
            {
                _logger.LogWarning($"Testing wireless device sensor ({sensor.Name} Pin Number: {sensor.Pin} Polarity: {sensor.Polarity})");
                var wirelessDevice = deviceConfiguration.WirelessDevices.First(x => x.Id == sensor.LocationId);

                _wirelessDeviceService.TriggerWirelessDeviceSensor(wirelessDevice, new List<WirelessDeviceSensor>()
                {
                    new WirelessDeviceSensor()
                    {
                        Pin = sensor.Pin,
                        Value = sensor.AlwaysOn ? GpioPinValue.Low : GpioPinValue.High,
                        InitialValue = sensor.AlwaysOn ? GpioPinValue.High : GpioPinValue.Low,
                        Length = TimeSpan.FromSeconds(maxTestRuntime).TotalMilliseconds
                    }
                });
                testRequest.StartTime = DateTime.UtcNow;
                Thread.Sleep(TimeSpan.FromSeconds(maxTestRuntime));
                testRequest.EndTime = DateTime.UtcNow;
                ResetDeviceSensors(new List<DeviceSensor>() { sensor });
            }
            _logger.LogWarning($"Testing sensor completed ({sensor.Name} Pin Number: {sensor.Pin} Polarity: {sensor.Polarity} Value: {endValue})");
            return testRequest;
        }
        public List<DeviceSensor> UpsertDeviceSensor(DeviceSensor deviceSensor)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            if (!deviceSensor.Id.HasValue)
            {
                var r = new Random();
                int? v = null;
                while (v == null || deviceConfiguration.Sensors.Any(tt => tt.Id == v))
                    v = r.Next(1000, 9999);
                deviceSensor.Id = v;
            }

            //Validate
            if (string.IsNullOrEmpty(deviceSensor.Name))
                throw new DeviceException($"Sensor must contain a valid name.");

            var sensors = deviceConfiguration.Sensors
                .Where(t => t.Id != deviceSensor.Id && t.Id.HasValue)
                .Select(s =>
                {
                    s.Value = null;
                    return s;
                })
                .ToList();
            deviceSensor.Value = null;
            sensors.Add(deviceSensor);
            deviceConfiguration.Sensors = sensors.OrderBy(s => s.Name).ToList();
            _logger.LogInformation($"Saving {sensors.Count()} device sensors to device...");
            _deviceConfiguration.SaveDeviceConfiguration(deviceConfiguration);
            Setup();
            return deviceConfiguration.Sensors;
        }
        public List<DeviceSensor> DeleteDeviceSensor(DeviceSensor deviceSensor)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();

            //Validate
            if (deviceConfiguration.Tasks.SelectMany(t => t.Actions).Any(a => a.TargetSensorId == deviceSensor.Id))
                throw new DeviceException("Device sensor is being used in a task action.");
            if (deviceConfiguration.Tasks.Any(t => t.TriggerSensorId == deviceSensor.Id))
                throw new DeviceException("Device sensor is being used in a task.");

            deviceConfiguration.Sensors = deviceConfiguration.Sensors.Where(t => t.Id != deviceSensor.Id).ToList();
            _deviceConfiguration.SaveDeviceConfiguration(deviceConfiguration);
            Setup();
            return deviceConfiguration.Sensors;
        }
        #endregion
        #region Schedule Archive CRUD
        public List<ScheduledJob> GetScheduledJobArchive()
        {
            var archivePath = _deviceConfiguration.LoadDeviceConfiguration()?.Settings.ScheduledJobArchivePath;
            try
            {
                var data = File.ReadAllText(archivePath);
                var archive = JsonConvert.DeserializeObject<List<ScheduledJob>>(data);
                return archive;
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"Could not read scheduled jobs archive.");
                return new List<ScheduledJob>();
            }
        }
        public List<ScheduledJob> ArchiveScheduledJob(ScheduledJob scheduledJob)
        {
            var archivePath = _deviceConfiguration.LoadDeviceConfiguration()?.Settings.ScheduledJobArchivePath;
            var archive = GetScheduledJobArchive();
            archive.Add(scheduledJob);
            File.WriteAllText(archivePath,JsonConvert.SerializeObject(archive));
            return archive;
        }
        public List<ScheduledJob> RemoveScheduledJobFromArchive(ScheduledJob scheduledJob)
        {
            var archivePath = _deviceConfiguration.LoadDeviceConfiguration()?.Settings.ScheduledJobArchivePath;
            var archive = GetScheduledJobArchive();
            archive = archive.Where(j => j.Id != scheduledJob.Id).ToList();
            File.WriteAllText(archivePath, JsonConvert.SerializeObject(archive));
            return archive;
        }
        #endregion
        #region Private Methods
        private List<ScheduledJob> ExpandTasks(DateTime startDate, DateTime endDate)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            var schedules = deviceConfiguration.Schedules;
            var allScheduledJobs = new List<ScheduledJob>();
            var startTime = startDate.TimeOfDay;
            var days = (endDate - startDate).TotalDays;
            for (var i = 0; i < days; i++)
                schedules.ForEach(schedule =>
                {
                    var scheduleTime = schedule.StartTime.ToLocalTime().TimeOfDay;
                    var scheduleDate = _dateTimeProvider.Now.Date.AddDays(i) + schedule.StartTime.ToLocalTime().TimeOfDay;
                    if (_dateTimeProvider.Now- scheduleDate > TimeSpan.FromSeconds(30))
                        return;
                    var time = startDate.Date.AddDays(i);
                    time += schedule.StartTime.ToLocalTime().TimeOfDay;
                    var offset = 0;
                    schedule.Tasks.ToList().ForEach(t =>
                    {
                        var task = deviceConfiguration.Tasks.First(tt => tt.Id == t.Id);

                        var r = new Random();
                        int? v = null;
                        while (v == null || allScheduledJobs.Any(tt => tt.Id == v))
                            v = r.Next(1000, 999999);

                        var scheduledJob = new ScheduledJob()
                        {
                            Id = v.Value,
                            TaskId = task.Id.Value,
                            StartTime = time.AddMinutes(offset)
                        };
                        offset += task.MaximumRuntime;
                        allScheduledJobs.Add(scheduledJob);
                    });
                });
            return allScheduledJobs.OrderBy(s => s.StartTime).ToList();
        }
        private async Task<ScheduledJob> DispatchJobStatus(ScheduledJob job)
        {
            try
            {
                var s = await _aquariumClient.DispatchScheduledJob(job);
                job.Id = s.Id;


                if (job.Status == JobStatus.Completed)
                {
                    /*
                    if (job.Task.TaskTypeId == ScheduleTaskTypes.StartATO)
                    {
                        await _aquariumClient.DispatchWaterATO(new ATOStatus()
                        {
                            StartTime = job.StartTime,
                            EndTime = job.EndTime.Value,
                            MlPerSec = 5,
                            ScheduleJobId = job.Id.Value
                        });
                    }
                    else if (job.Task.TaskTypeId == ScheduleTaskTypes.WaterChangeReplentish)
                    {
                        var previousJob = job;
                        while (previousJob.PreviousJob != null)
                            previousJob = previousJob.PreviousJob;
                        await _aquariumClient.DispatchWaterChange(new WaterChange()
                        {
                            StartTime = previousJob.StartTime,
                            EndTime = job.EndTime.Value,
                            ScheduleJobId = job.Id.Value
                        });
                    }
                    */
                }

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

        #endregion
    }
}
