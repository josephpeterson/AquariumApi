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
        DeviceSensor GetFloatSensorPin();
        DeviceSensor GetPumpRelayPin();
        void Setup();
        void StopAutoTopOff(AutoTopOffStopReason stopReason = AutoTopOffStopReason.ForceStop);
    }
    public class ATOService : IATOService
    {
        public ATOStatus Status { get; set; }
        public AutoTopOffRequest Request { get; set; }

        private CancellationTokenSource CancelToken;
        private IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;
        private readonly ILogger<ATOService> _logger;
        private readonly IGpioService _gpioService;
        private IAquariumClient _aquariumClient;

        public ATOService(IConfiguration config, ILogger<ATOService> logger, IGpioService gpioService, IHostingEnvironment hostingEnvironment,IAquariumClient aquariumClient)
        {
            _config = config;
            _logger = logger;
            _gpioService = gpioService;
            _aquariumClient = aquariumClient;
        }


        public void Setup()
        {
            var pumpRelaySensor = GetPumpRelayPin();
            var floatSwitchSensor = GetFloatSensorPin();
            if (pumpRelaySensor == null || floatSwitchSensor == null)
                throw new Exception($"Invalid ATO pins specified (Pump: {pumpRelaySensor} Sensor: {floatSwitchSensor}");
            floatSwitchSensor.OnSensorTriggered = OnFloatSwitchTriggered;
            _logger.LogInformation("ATO successfully set up");
        }
        public DeviceSensor GetFloatSensorPin()
        {
            var sensors = _gpioService.GetAllSensors();
            return sensors.Where(p => p.Type == SensorTypes.FloatSwitch).FirstOrDefault();
        }
        public DeviceSensor GetPumpRelayPin()
        {
            var sensors = _gpioService.GetAllSensors();
            return sensors.Where(p => p.Type == SensorTypes.ATOPumpRelay).FirstOrDefault();
        }
        public void BeginAutoTopOff(AutoTopOffRequest atoRequest)
        {
            if (CancelToken != null && !CancelToken.IsCancellationRequested)
                CancelToken.Cancel();

            var pumpRelaySensor = GetPumpRelayPin();
            var floatSwitchSensor = GetFloatSensorPin();
            if(pumpRelaySensor == null || floatSwitchSensor == null)
                throw new Exception($"Invalid ATO pins specified (Pump: {pumpRelaySensor} Sensor: {floatSwitchSensor}");

            Request = atoRequest;
            Status = new ATOStatus()
            {
                StartTime = Request.StartTime,
                EstimatedEndTime = Request.StartTime.AddMinutes(24),
                MaxRuntime = Request.Runtime,
                RunIndefinitely = Request.RunIndefinitely
            };

            //Attempt to tell AquariumApi we are running
            Status = _aquariumClient.DispatchATOStatus(Status).Result;

            _logger.LogInformation("[ATOService] Beginning ATO...");
            _gpioService.SetPinValue(pumpRelaySensor, PinValue.High);

            //Apply a max drain time
            var maxPumpRuntime = Status.MaxRuntime * 1000 * 60;
            CancelToken = new CancellationTokenSource();
            CancellationToken ct = CancelToken.Token;

            Task.Run(() =>
            {
                Status.PumpRunning = true;
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

            var pumpRelaySensor = GetPumpRelayPin();
            var floatSwitchSensor = GetFloatSensorPin();
            if (pumpRelaySensor == null || floatSwitchSensor == null)
                throw new Exception($"Invalid ATO pins specified (Pump: {pumpRelaySensor} Sensor: {floatSwitchSensor}");
            _gpioService.SetPinValue(pumpRelaySensor, PinValue.Low);


            Status.PumpRunning = false;
            Status.EndReason = stopReason.ToString();
            Status.ActualEndTime = new DateTime();
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
        public ATOStatus GetATOStatus() => Status;
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
