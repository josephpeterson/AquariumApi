using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IATOService: IDeviceSetupService
    {
        void BeginAutoTopOff(AutoTopOffRequest atoRequest);
        ATOStatus GetATOStatus();
        void StopAutoTopOff(AutoTopOffStopReason stopReason = AutoTopOffStopReason.ForceStop);
        void BeginWaterChange(WaterChangeRequest atoRequest);
        void StopWaterChange(AutoTopOffStopReason canceled);
    }
    public class ATOService : IATOService
    {
        public AquariumDevice Device { get; set; }
        public ATOStatus Status = new ATOStatus();
        public AutoTopOffRequest Request { get; set; }

        private CancellationTokenSource CurrentTaskToken;
        private readonly IConfiguration _config;
        private readonly ILogger<ATOService> _logger;
        private readonly IAquariumAuthService _aquariumAuthService;
        private readonly IGpioService _gpioService;
        private readonly ScheduleService _scheduleService;
        private IAquariumClient _aquariumClient;
        private IExceptionService _exceptionService;

        public ATOService(IConfiguration config, ILogger<ATOService> logger, 
            IAquariumAuthService aquariumAuthService,
            IGpioService gpioService, 
            ScheduleService scheduleService,
            IExceptionService exceptionService,
            IAquariumClient aquariumClient)
        {
            _config = config;
            _logger = logger;
            _aquariumAuthService = aquariumAuthService;
            _gpioService = gpioService;
            _scheduleService = scheduleService;
            _aquariumClient = aquariumClient;
            _exceptionService = exceptionService;
        }


        public void Setup()
        {
            var device = _aquariumAuthService.GetAquarium().Device;

            //Register associated device tasks
            RegisterDeviceTasks();

            //Check pins and sensors
            var pumpRelaySensor = GetPumpRelayPin();
            var floatSwitchSensor = GetFloatSensorPin();
            if (pumpRelaySensor == null || floatSwitchSensor == null)
                throw new Exception($"Invalid ATO sensors (Pump: {pumpRelaySensor} Sensor: {floatSwitchSensor})");
            floatSwitchSensor.OnSensorTriggered = OnFloatSwitchTriggered;

            var task = device.ScheduleAssignments.Select(assignment => 
            assignment.Schedule.Tasks.Where(t => t.TaskId == ScheduleTaskTypes.StartATO).FirstOrDefault()
            ).FirstOrDefault();

            DateTime? nextRunTime = null;
            if(task != null)
                nextRunTime = task.StartTime.ToUniversalTime();

            _logger.LogInformation($"ATO successfully set up (Pump Relay Pin: {pumpRelaySensor.Pin} Float Sensor Pin: {floatSwitchSensor.Pin})");
            Status = new ATOStatus()
            {
                PumpRelaySensor = pumpRelaySensor,
                FloatSensor = floatSwitchSensor,
                Enabled = true,
                DeviceId = device.Id,
                UpdatedAt = new DateTime(),
                NextRunTime = nextRunTime
            };
        }
        private DeviceSensor GetFloatSensorPin()
        {
            var sensors = _gpioService.GetAllSensors();
            return sensors.Where(p => p.Type == SensorTypes.FloatSwitch).FirstOrDefault();
        }
        private DeviceSensor GetPumpRelayPin()
        {
            var sensors = _gpioService.GetAllSensors();
            return sensors.Where(p => p.Type == SensorTypes.ATOPumpRelay).FirstOrDefault();
        }
        private DeviceSensor GetWaterChangeDrainPin()
        {
            var sensors = _gpioService.GetAllSensors();
            return sensors.Where(p => p.Type == SensorTypes.Solenoid).FirstOrDefault();
        }
        private DeviceSensor GetWaterChangeReplentishPin()
        {
            var sensors = _gpioService.GetAllSensors();
            return sensors.Where(p => p.Type == SensorTypes.WaterChangePumpRelay).FirstOrDefault();
        }

        private async Task DispatchStatus()
        {
            try
            {
                var s = await _aquariumClient.DispatchATOStatus(Status);
                Status.Id = s.Id;
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to dispatch ATO status to server");
                _logger.LogError(e.Message);
                _logger.LogError(e.StackTrace);
                await _exceptionService.Throw(e);
            }
        }

        public void BeginAutoTopOff(AutoTopOffRequest atoRequest)
        {
            if (Status.PumpRunning)
            {
                throw new Exception($"ATO is currently running...");
            }
            if (CurrentTaskToken != null && !CurrentTaskToken.IsCancellationRequested)
            {
                _logger.LogInformation("Canceled by new ATO process"); //todo remove all these cancel logs when we solve the issue of random ato cancels
                CurrentTaskToken.Cancel();
            }
            var pumpRelaySensor = GetPumpRelayPin();
            var floatSwitchSensor = GetFloatSensorPin();
            if (pumpRelaySensor == null || floatSwitchSensor == null)
                throw new Exception($"Invalid ATO pins specified (Pump: {pumpRelaySensor} Sensor: {floatSwitchSensor})");


            //Check if they passed a number larger than the maximum ATO time allowed
            var maxATOTime = Convert.ToInt32(_config["ATOMaximumRuntime"]);
            if (atoRequest.Runtime > maxATOTime) //max run time
                throw new Exception($"ATO max runtime is larger than maximum allowed (Runtime: {atoRequest.Runtime} Maximum: {maxATOTime})");

            var currentSensorValue = _gpioService.GetPinValue(floatSwitchSensor);

            if(currentSensorValue == GpioPinValue.Low)
                throw new Exception($"ATO sensor is currently reading maximum water level");




            _logger.LogInformation("[ATOService] Beginning ATO...");
            var maxPumpRuntime = atoRequest.Runtime * 1000 * 60;
            _gpioService.SetPinValue(pumpRelaySensor, PinValue.High);

            //Next run time
            var device = _aquariumAuthService.GetAquarium().Device;
            var task = device.ScheduleAssignments.Select(assignment =>
            assignment.Schedule.Tasks.Where(t => t.TaskId == Models.ScheduleTaskTypes.StartATO).FirstOrDefault()
            ).FirstOrDefault();
            DateTime? nextRunTime = null;
            if (task != null)
                nextRunTime = task.StartTime.ToUniversalTime();

            var startTime = DateTime.Now.ToUniversalTime();
            Status = new ATOStatus
            {
                StartTime = startTime,
                EstimatedEndTime = startTime.AddMilliseconds(maxPumpRuntime),
                UpdatedAt = startTime,
                MaxRuntime = atoRequest.Runtime,
                RunIndefinitely = atoRequest.RunIndefinitely,
                PumpRunning = true,
                Enabled = true,
                PumpRelaySensor = pumpRelaySensor,
                FloatSensor = floatSwitchSensor,
                DeviceId = device.Id,
                NextRunTime = nextRunTime,
                FloatSensorValue = currentSensorValue,
                MlPerSec = 1.25 //Calculated 150ml in 2min = 1.2mlps
            };
            DispatchStatus().Wait(); //.ConfigureAwait(false);

            //Apply a max drain time
            CurrentTaskToken = new CancellationTokenSource();
            CancellationToken ct = CurrentTaskToken.Token;


            var statusId = Status.Id;
            Task.Run(() =>
            {
                var t = TimeSpan.FromMilliseconds(maxPumpRuntime);
                string time = string.Format("{0:D2}h:{1:D2}m", t.Hours, t.Minutes);
                _logger.LogInformation($"[ATOService] Running (Max run time: {time})");
                Thread.Sleep(maxPumpRuntime);
                //ct.ThrowIfCancellationRequested();
                if (ct.IsCancellationRequested)
                {
                    if(statusId != Status.Id)
                    {
                        _logger.LogError("Attempted to cancel water change. The Status IDs of the water changes do not match!");
                        //so this definitely happens occasionally 
                        return;
                    }
                    StopAutoTopOff(AutoTopOffStopReason.Canceled);
                    //_gpioService.SetPinValue(pumpRelaySensor.Pin, PinValue.Low); //maybe enable this in case we run into pin issues
                    return;
                }

                if (Status.PumpRunning)
                {
                    _logger.LogInformation($"[ATOService] Reached maximum run time of {Status.MaxRuntime} minutes.");
                    StopAutoTopOff(AutoTopOffStopReason.MaximumRuntimeReached);
                }

            }).ConfigureAwait(false); //Fire and forget
        }
        public void StopAutoTopOff(AutoTopOffStopReason stopReason = AutoTopOffStopReason.ForceStop)
        {
            if (CurrentTaskToken != null && !CurrentTaskToken.IsCancellationRequested)
            {
                _logger.LogInformation("Canceled via StopAutoTopOff");
                CurrentTaskToken.Cancel();
            }


            var pumpRelaySensor = Status.PumpRelaySensor;
            var floatSwitchSensor = Status.FloatSensor;
            if (pumpRelaySensor == null || floatSwitchSensor == null)
                throw new Exception($"Invalid ATO pins specified (Pump: {pumpRelaySensor} Sensor: {floatSwitchSensor})");
            _gpioService.SetPinValue(pumpRelaySensor, PinValue.Low);

            if (!Status.PumpRunning)
                return;


            Status.PumpRunning = false;
            Status.EndReason = stopReason.ToString();
            Status.ActualEndTime = DateTime.Now.ToUniversalTime();
            Status.UpdatedAt = Status.ActualEndTime;
            Status.Completed = true;
            Status.FloatSensorValue = _gpioService.GetPinValue(floatSwitchSensor);
            DispatchStatus().Wait(); //.ConfigureAwait(false);

            _logger.LogInformation($"[ATOService] ATO Stopped! ({stopReason})");

        }
        public void OnFloatSwitchTriggered(object sender,int value)
        {
            Status.FloatSensorValue = _gpioService.GetPinValue(Status.FloatSensor);

            _logger.LogInformation($"ATOService: Sensor triggered (Value: {Status.FloatSensorValue})");
            if (Status.PumpRunning && Status.FloatSensorValue == GpioPinValue.Low)
            {
                _logger.LogInformation($"[ATOService] ATO Stopped!");
                StopAutoTopOff(AutoTopOffStopReason.SensorTriggered);
            }
        }
        public ATOStatus GetATOStatus()
        {
            Status.UpdatedAt = DateTime.Now.ToUniversalTime();
            Status.FloatSensorValue = _gpioService.GetPinValue(Status.FloatSensor);

            var nextRun = _scheduleService.GetAllScheduledTasks().Where(t => t.TaskId == ScheduleTaskTypes.StartATO).FirstOrDefault();
            if (nextRun != null)
                Status.NextRunTime = nextRun.StartTime;
            return Status;
        }

        public void CleanUp()
        {
            var pumpRelay = GetPumpRelayPin();
            if (pumpRelay != null && Status.PumpRunning)
                _gpioService.SetPinValue(pumpRelay, PinValue.Low);
            if (CurrentTaskToken != null && !CurrentTaskToken.IsCancellationRequested)
            {
                _logger.LogInformation("Canceled via CleanUp");
                CurrentTaskToken.Cancel();
            }
        }
        private void RegisterDeviceTasks()
        {
            var scheduledRuntime = Convert.ToInt32(_config["ATOScheduledRuntime"]);
            _logger.LogInformation($"Registering ScheduleTaskType StartATO with scheduled runtime: {scheduledRuntime}");
            _scheduleService.RegisterDeviceTask(ScheduleTaskTypes.StartATO, (DeviceScheduleTask t) =>
            {
                BeginAutoTopOff(new AutoTopOffRequest()
                {
                    Runtime = scheduledRuntime //standard ATO time
                });
            });
            _logger.LogInformation($"Registering ScheduleTaskType StartWaterChange with scheduled runtime: {scheduledRuntime}");
            _scheduleService.RegisterDeviceTask(ScheduleTaskTypes.StartWaterChange, (DeviceScheduleTask t) =>
            {
                BeginWaterChange(new WaterChangeRequest()
                {
                    DrainTime = scheduledRuntime, //standard ATO time
                    ReplentishTime = scheduledRuntime
                });
            });
        }

        public void BeginWaterChange(WaterChangeRequest atoRequest)
        {
            if (Status.PumpRunning)
            {
                throw new Exception($"ATO is currently running...");
            }
            if (CurrentTaskToken != null && !CurrentTaskToken.IsCancellationRequested)
            {
                _logger.LogInformation("Canceled by new ATO process"); //todo remove all these cancel logs when we solve the issue of random ato cancels
                CurrentTaskToken.Cancel();
            }

            var replentishPump = GetWaterChangeReplentishPin();
            var drainPump = GetWaterChangeDrainPin();
            var floatSwitchSensor = GetFloatSensorPin();
            if (replentishPump == null || floatSwitchSensor == null || drainPump == null)
                throw new Exception($"Invalid water change pins specified (Replentish Pump: {replentishPump}, Drain Pump: {drainPump}, Float Sensor: {floatSwitchSensor})");


            //Check if they passed a number larger than the maximum ATO time allowed
            var maxATOTime = Convert.ToInt32(_config["WaterChangeMaximumRuntime"]);
            if (atoRequest.ReplentishTime > maxATOTime) //max run time
                throw new Exception($"Water change replentish time is higher than maxium allowed (Runtime: {atoRequest.ReplentishTime} Maximum: {maxATOTime})");
            if (atoRequest.DrainTime > maxATOTime) //max run time
                throw new Exception($"Water change drain time is higher than maxium allowed (Runtime: {atoRequest.DrainTime} Maximum: {maxATOTime})");

            var currentSensorValue = _gpioService.GetPinValue(floatSwitchSensor);
            if (currentSensorValue == GpioPinValue.Low)
                throw new Exception($"ATO sensor is currently reading maximum water level");


            _logger.LogInformation("[ATOService] Beginning Water Change...");
            var maxReplentishPumpRuntime = atoRequest.ReplentishTime * 1000 * 60;
            var maxDrainPumpRuntime = atoRequest.DrainTime * 1000 * 60;



            //Next run time
            var device = _aquariumAuthService.GetAquarium().Device;
            var task = device.ScheduleAssignments.Select(assignment =>
            assignment.Schedule.Tasks.Where(t => t.TaskId == Models.ScheduleTaskTypes.StartWaterChange).FirstOrDefault()
            ).FirstOrDefault();
            DateTime? nextRunTime = null;
            if (task != null)
                nextRunTime = task.StartTime.ToUniversalTime();

            var startTime = DateTime.Now.ToUniversalTime();
            Status = new ATOStatus
            {
                StartTime = startTime,
                EstimatedEndTime = startTime.AddMilliseconds(maxReplentishPumpRuntime),
                UpdatedAt = startTime,
                MaxRuntime = atoRequest.DrainTime,
                RunIndefinitely = false,
                PumpRunning = true,
                Enabled = true,
                PumpRelaySensor = replentishPump,
                FloatSensor = floatSwitchSensor,
                DeviceId = device.Id,
                NextRunTime = nextRunTime,
                FloatSensorValue = currentSensorValue,
                MlPerSec = 1.25 //Calculated 150ml in 2min = 1.2mlps
            };
            DispatchStatus().Wait(); //.ConfigureAwait(false);

            //Apply a max drain time
            CurrentTaskToken = new CancellationTokenSource();
            CancellationToken ct = CurrentTaskToken.Token;


            var statusId = Status.Id;
            Task.Run(() =>
            {
                _logger.LogWarning("[ATOService] Opening water change drain...");
                _gpioService.SetPinValue(drainPump, PinValue.High);
                Thread.Sleep(TimeSpan.FromMilliseconds(maxDrainPumpRuntime));

                _logger.LogWarning("[ATOService] Closing water change drain...");
                _gpioService.SetPinValue(drainPump, PinValue.Low);

                _logger.LogWarning("[ATOService] Starting replenish pump...");
                _gpioService.SetPinValue(replentishPump, PinValue.High);
                Thread.Sleep(TimeSpan.FromMilliseconds(maxReplentishPumpRuntime));

                _logger.LogWarning("[ATOService] Stopping replentish pump...");

                if (ct.IsCancellationRequested)
                {
                    if (statusId != Status.Id)
                    {
                        _logger.LogError("Attempted to cancel water change. The Status IDs of the water changes do not match!");
                        //so this definitely happens occasionally 
                        return;
                    }
                    StopWaterChange(AutoTopOffStopReason.Canceled);
                    //_gpioService.SetPinValue(pumpRelaySensor.Pin, PinValue.Low); //maybe enable this in case we run into pin issues
                    return;
                }

                if (Status.PumpRunning)
                {
                    _logger.LogInformation($"[ATOService] Reached maximum run time of {Status.MaxRuntime} minutes.");
                    StopWaterChange(AutoTopOffStopReason.MaximumRuntimeReached);
                }

            }).ConfigureAwait(false); //Fire and forget
        }

        public void StopWaterChange(AutoTopOffStopReason stopReason)
        {
            if (CurrentTaskToken != null && !CurrentTaskToken.IsCancellationRequested)
            {
                _logger.LogInformation("Canceled via StopWaterChange");
                CurrentTaskToken.Cancel();
            }

            var replentishPump = GetWaterChangeReplentishPin();
            var drainPump = GetWaterChangeDrainPin();
            var floatSwitchSensor = GetFloatSensorPin();
            if (replentishPump == null || floatSwitchSensor == null || drainPump == null)
                throw new Exception($"Invalid water change pins specified (Replentish Pump: {replentishPump}, Drain Pump: {drainPump}, Float Sensor: {floatSwitchSensor})");
            _gpioService.SetPinValue(replentishPump, PinValue.Low);
            _gpioService.SetPinValue(drainPump, PinValue.Low);

            if (!Status.PumpRunning)
                return;


            Status.PumpRunning = false;
            Status.EndReason = stopReason.ToString();
            Status.ActualEndTime = DateTime.Now.ToUniversalTime();
            Status.UpdatedAt = Status.ActualEndTime;
            Status.Completed = true;
            Status.FloatSensorValue = _gpioService.GetPinValue(floatSwitchSensor);
            DispatchStatus().Wait(); //.ConfigureAwait(false);

            _logger.LogInformation($"[ATOService] ATO Stopped! ({stopReason})");
        }
    }
    public class AutoTopOffRequest
    {
        public DateTime StartTime { get; set; }
        public int Runtime { get; set; }
        public bool RunIndefinitely { get; set; }
    }
    public class WaterChangeRequest
    {
        public DateTime StartTime { get; set; }
        public int ReplentishTime { get; set; }
        public int DrainTime { get; set; }
    }
    public enum AutoTopOffStopReason
    {
        Error,
        Canceled,
        ForceStop,
        MaximumRuntimeReached,
        SensorTriggered,
    }
}
