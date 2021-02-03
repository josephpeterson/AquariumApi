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
    public interface IATOService
    {
        void BeginAutoTopOff(AutoTopOffRequest atoRequest);
        ATOStatus GetATOStatus();
        void Setup(AquariumDevice device);
        void StopAutoTopOff(AutoTopOffStopReason stopReason = AutoTopOffStopReason.ForceStop);
    }
    public class ATOService : IATOService
    {
        public AquariumDevice Device { get; set; }
        public ATOStatus Status = new ATOStatus();
        public AutoTopOffRequest Request { get; set; }

        private CancellationTokenSource CancelToken;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;
        private readonly ILogger<ATOService> _logger;
        private readonly IGpioService _gpioService;
        private IAquariumClient _aquariumClient;
        private AquariumDevice _device;

        public ATOService(IConfiguration config, ILogger<ATOService> logger, IGpioService gpioService, IHostingEnvironment hostingEnvironment,IAquariumClient aquariumClient)
        {
            _config = config;
            _logger = logger;
            _gpioService = gpioService;
            _aquariumClient = aquariumClient;
        }


        public void Setup(AquariumDevice device)
        {
            _device = device;

            //Check pins and sensors
            var pumpRelaySensor = GetPumpRelayPin();
            var floatSwitchSensor = GetFloatSensorPin();
            if (pumpRelaySensor == null || floatSwitchSensor == null)
                throw new Exception($"Invalid ATO sensors (Pump: {pumpRelaySensor} Sensor: {floatSwitchSensor})");
            floatSwitchSensor.OnSensorTriggered = OnFloatSwitchTriggered;

            var task = device.ScheduleAssignments.Select(assignment => 
            assignment.Schedule.Tasks.Where(t => t.TaskId == Models.ScheduleTaskTypes.StartATO).FirstOrDefault()
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
                DeviceId = _device.Id,
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

        public void BeginAutoTopOff(AutoTopOffRequest atoRequest)
        {
            if (CancelToken != null && !CancelToken.IsCancellationRequested)
            {
                throw new Exception($"ATO is currently running...");
            }
            var pumpRelaySensor = GetPumpRelayPin();
            var floatSwitchSensor = GetFloatSensorPin();
            if (pumpRelaySensor == null || floatSwitchSensor == null)
                throw new Exception($"Invalid ATO pins specified (Pump: {pumpRelaySensor} Sensor: {floatSwitchSensor})");

            if (atoRequest.Runtime > 60)
                throw new Exception($"ATO max runtime is larger than maximum allowed (Runtime: {atoRequest.Runtime} Maximum: 60)");

            var currentSensorValue = _gpioService.GetPinValue(floatSwitchSensor);

            if(currentSensorValue == GpioPinValue.Low)
                throw new Exception($"ATO sensor is currently reading maximum water level");


            _logger.LogInformation("[ATOService] Beginning ATO...");
            _gpioService.SetPinValue(pumpRelaySensor, PinValue.High);

            var maxPumpRuntime = atoRequest.Runtime * 1000 * 60;
            var task = _device.ScheduleAssignments.Select(assignment =>
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
                DeviceId = _device.Id,
                NextRunTime = nextRunTime,
                FloatSensorValue = currentSensorValue
            };
            DispatchStatus().Wait(); //.ConfigureAwait(false);

            //Apply a max drain time
            CancelToken = new CancellationTokenSource();
            CancellationToken ct = CancelToken.Token;

            Task.Run(() =>
            {
                _logger.LogInformation($"[ATOService] Running (Max run time: {maxPumpRuntime}ms)");
                Thread.Sleep(maxPumpRuntime);
                //ct.ThrowIfCancellationRequested();
                if (ct.IsCancellationRequested)
                {
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
            if (CancelToken != null && !CancelToken.IsCancellationRequested)
                CancelToken.Cancel();


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

            _logger.LogInformation($"ATOService: Sensor triggered (Value: {Status.FloatSensorValue}");
            if (Status.PumpRunning && Status.FloatSensorValue == GpioPinValue.High)
            {
                _logger.LogInformation($"[ATOService] ATO Stopped!");
                StopAutoTopOff(AutoTopOffStopReason.SensorTriggered);
            }
        }
        public ATOStatus GetATOStatus()
        {
            Status.UpdatedAt = DateTime.Now.ToUniversalTime();
            Status.FloatSensorValue = _gpioService.GetPinValue(Status.FloatSensor);
            return Status;
        }
        private async Task DispatchStatus() {
            try
            {
                var s = await _aquariumClient.DispatchATOStatus(Status);
                Status.Id = s.Id;
            }
            catch(Exception e)
            {
                _logger.LogError("Unable to dispatch ATO status to server");
            }
        }
    }
    public class AutoTopOffRequest
    {
        public DateTime StartTime { get; set; }
        public int Runtime { get; set; }
        public bool RunIndefinitely { get; set; }
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
