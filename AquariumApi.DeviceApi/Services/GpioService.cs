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
    public interface IGpioService
    {
        List<DeviceSensor> GetSensorValues();
        void WaterChangeBeginDrain();
        void WaterChangeDrainTest(int msLength);
    } 
    public class GpioService : IGpioService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HardwareService> _logger;
        private readonly ISerialService _serialService;
        private readonly IHostingEnvironment _hostingEnvironment;


        private int solenoidPin = 21;
        private int drainCompleteSensorPin = 20;
        private CancellationTokenSource maxDrainLimitReachedTaskCancelationToken;

        private List<DeviceSensor> pins = new List<DeviceSensor>()
        {
            new  DeviceSensor()
            {
                Name = "Solenoid Valve",
                Pin = 21,
                Type = SensorTypes.Solenoid,
                Polarity = Polarity.Output
            },
            new  DeviceSensor()
            {
                Name = "Drain Complete Sensor",
                Pin = 20,
                Type = SensorTypes.FloatSwitch,
                Polarity = Polarity.Input
            },
        };


        private bool Draining = false;

        private GpioController Controller;


        public GpioService(IConfiguration config, ILogger<HardwareService> logger,ISerialService serialService,IHostingEnvironment hostingEnvironment)
        {
            _config = config;
            _logger = logger;
            _serialService = serialService;
            _hostingEnvironment = hostingEnvironment;


            try
            {
                Controller = new GpioController();
                _logger.LogInformation("GpioService: Initiated GpioController successfully.");

                _logger.LogInformation("GpioService: Preparing pins....");
                PreparePins();
            }
            catch(Exception ex)
            {
                _logger.LogError("GpioService: Encountered an error\r\n");
                _logger.LogError(ex.StackTrace);
            }
        }
        public void PreparePins()
        {
            pins.ForEach(p =>
            {
                if (!Controller.IsPinOpen(p.Pin))
                {
                    Controller.OpenPin(p.Pin, p.Polarity == 0 ? PinMode.InputPullUp : PinMode.Output);
                    

                    if(p.Polarity == 0)
                        Controller.RegisterCallbackForPinValueChangedEvent(p.Pin, PinEventTypes.Falling, OnSensorTriggered);

                }
                else
                    _logger.LogInformation("GpioService: PreparePins: Pin is already open (" + p.Name + ")");
            });
        }
        public void CleanUp()
        {
            pins.ForEach(p =>
            {
                if (Controller.IsPinOpen(p.Pin))
                    Controller.ClosePin(p.Pin);
            });
        }
        public void WaterChangePrepare()
        {
            //Turn off return pump and Protein Skimmer

        }
        public List<DeviceSensor> GetSensorValues()
        {
            var inputPins = pins.Where(p => p.Polarity == Polarity.Input).ToList();
            inputPins.ForEach(p => {
                var val = Controller.Read(p.Pin);
                _logger.LogInformation("Pin Value for " + p.Name + ": " + val);
                p.Value = val.ToString();
            });
            return inputPins;
        }
        public void WaterChangeBeginDrain()
        {
            if (IsDraining())
                throw new Exception("Water is already draining.");

            _logger.LogInformation("[WaterChange] ATTEMPTING TO DRAIN WATER");

            var drainCompleteSensor = pins[1];

            var val = Controller.Read(drainCompleteSensor.Pin);

            if (val == PinValue.Low)
                throw new Exception("The water level is already at it's lowest point. Could not drain anymore water.");

            OpenDrainHose();
        }





        private void CloseDrainHose()
        {
            if (maxDrainLimitReachedTaskCancelationToken != null && !maxDrainLimitReachedTaskCancelationToken.IsCancellationRequested)
                maxDrainLimitReachedTaskCancelationToken.Cancel();

            Controller.Write(solenoidPin, PinValue.Low);
            Draining = false;
            _logger.LogInformation("[WaterChange] Drain closed");
        }
        private void OpenDrainHose()
        {
            if (maxDrainLimitReachedTaskCancelationToken != null && !maxDrainLimitReachedTaskCancelationToken.IsCancellationRequested)
                maxDrainLimitReachedTaskCancelationToken.Cancel();

            //Open solenoid valve
            _logger.LogInformation("[WaterChange] SOLENOID OPEN");
            Controller.Write(solenoidPin, PinValue.High);
            Draining = true;


            //Apply a max drain time
            var maxDrainTime = 10 * 1000;
            maxDrainLimitReachedTaskCancelationToken = new CancellationTokenSource();
            CancellationToken ct = maxDrainLimitReachedTaskCancelationToken.Token;
            Task.Run(() =>
            {
                Thread.Sleep(maxDrainTime);
                ct.ThrowIfCancellationRequested();
                if (ct.IsCancellationRequested)
                    return;

                if (IsDraining())
                    CloseDrainHose();

            }).ConfigureAwait(false); //Fire and forget
        }
        private bool IsDraining()
        {
            return Draining;
        }




        private void OnSensorTriggered(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            var drainCompleteSensor = pins[1];
            if (pinValueChangedEventArgs.PinNumber != drainCompleteSensor.Pin)
                return;

            if(IsDraining())
                CloseDrainHose();
        }

       
        public void WaterChangeDrainTest(int msLength)
        {
            if (IsDraining())
            {
                CloseDrainHose();
                Thread.Sleep(500);
            }

            OpenDrainHose();
            Thread.Sleep(msLength);
            CloseDrainHose();
        }
        public void WaterChangeBeginReplenish()
        {
            //Turn on replenish pump until sensor detects high water level
        }
        public void WaterChangeBeginATO()
        {
            //Turn on ATO pump until sensor detects high water level
        }
    }
}
