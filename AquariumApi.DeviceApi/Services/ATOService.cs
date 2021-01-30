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
        void SetATOStatusId(int id);
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
                CancelToken.Cancel();

            if(Status.PumpRelaySensor == null || Status.FloatSensor == null)
                throw new Exception($"Invalid ATO pins specified (Pump: {Status.PumpRelaySensor} Sensor: {Status.FloatSensor}");

            _logger.LogInformation("[ATOService] Beginning ATO...");
            _gpioService.SetPinValue(Status.PumpRelaySensor, PinValue.High);

            Request = atoRequest;
            Status.Id = null;
            Status.StartTime = DateTime.Now.ToUniversalTime();
            Status.EstimatedEndTime = Status.StartTime.AddMinutes(24);
            Status.MaxRuntime = Request.Runtime;
            Status.RunIndefinitely = Request.RunIndefinitely;
            Status.UpdatedAt = Status.StartTime;
            Status.PumpRunning = true;
            Status.Completed = false;
            DispatchStatus().ConfigureAwait(false);

            //Apply a max drain time
            var maxPumpRuntime = Status.MaxRuntime * 1000 * 60;
            CancelToken = new CancellationTokenSource();
            CancellationToken ct = CancelToken.Token;

            Task.Run(() =>
            {
                Thread.Sleep(maxPumpRuntime);
                ct.ThrowIfCancellationRequested();
                if (ct.IsCancellationRequested)
                {
                    StopAutoTopOff(AutoTopOffStopReason.ForceStop);
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
                throw new Exception($"Invalid ATO pins specified (Pump: {pumpRelaySensor} Sensor: {floatSwitchSensor}");
            _gpioService.SetPinValue(pumpRelaySensor, PinValue.Low);


            Status.PumpRunning = false;
            Status.EndReason = stopReason.ToString();
            Status.ActualEndTime = DateTime.Now.ToUniversalTime();
            Status.UpdatedAt = Status.ActualEndTime;
            Status.Completed = true;

            if (Status.Id.HasValue) //this tells us we've recieved a response from the server
                _aquariumClient.DispatchATOStatus(Status);

            _logger.LogInformation($"[ATOService] ATO Stopped! ({stopReason})");

        }
        public void OnFloatSwitchTriggered(object sender,int value)
        {
            Status.FloatSensorValue = $"{value}";
            if (Status.PumpRunning)
            {
                _logger.LogInformation($"[ATOService] ATO Stopped!");
                StopAutoTopOff(AutoTopOffStopReason.SensorTriggered);
            }
        }
        public ATOStatus GetATOStatus()
        {
            Status.UpdatedAt = DateTime.Now.ToUniversalTime();
            return Status;
        }
        public void SetATOStatusId(int id)
        {
            Status.Id = id;
        }
        private async Task DispatchStatus() {
            try
            {
                Status = await _aquariumClient.DispatchATOStatus(Status);
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
        ForceStop,
        MaximumRuntimeReached,
        SensorTriggered,
    }
}
