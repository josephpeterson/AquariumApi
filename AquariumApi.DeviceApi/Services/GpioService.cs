using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
        void CleanUp();
        List<DeviceSensor> GetAllSensors();
        GpioPinValue GetPinValue(DeviceSensor pin);
        void RegisterDevicePin(DeviceSensor deviceSensor);
        void SetPinValue(DeviceSensor pin, GpioPinValue pinValue);
        void Setup();
    }
    public class GpioService : IGpioService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<GpioService> _logger;
        private readonly ISerialService _serialService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IDeviceConfigurationService _deviceConfiguration;
        private readonly List<DeviceSensor> Pins = new List<DeviceSensor>();
        private IGpioControllerWrapper Controller;
        public bool ATOSystemRunning { get; private set; }

        public GpioService(IConfiguration config
            ,ILogger<GpioService> logger
            ,ISerialService serialService
            , IWebHostEnvironment hostingEnvironment
            ,IDeviceConfigurationService deviceConfiguration
            )
        {
            _config = config;
            _logger = logger;
            _serialService = serialService;
            _hostingEnvironment = hostingEnvironment;
            _deviceConfiguration = deviceConfiguration;
            CreateController();
        }
        private void PreparePins()
        {
            Pins.ForEach(p =>
            {
                if (!Controller.IsPinOpen(p.Pin))
                {
                    Controller.OpenPin(p.Pin, p.Polarity == 0 ? PinMode.InputPullUp : PinMode.Output);
                    p.Value = GpioPinValue.Low;

                    if (p.Polarity == 0) // Input only
                    {
                        Controller.RegisterCallbackForPinValueChangedEvent(p.Pin, PinEventTypes.Falling, (object sender, PinValueChangedEventArgs pinValueChangedEventArgs) =>
                            OnDeviceSensorTriggered(p,sender,pinValueChangedEventArgs)
                        );
                        Controller.RegisterCallbackForPinValueChangedEvent(p.Pin, PinEventTypes.Rising, (object sender, PinValueChangedEventArgs pinValueChangedEventArgs) =>
                            OnDeviceSensorTriggered(p, sender, pinValueChangedEventArgs)
                        );
                        p.Value = Controller.Read(p.Pin);
                    }

                    //default always on
                    if (p.AlwaysOn)
                    {
                        p.Value = GpioPinValue.High;
                        Controller.Write(p.Pin, GpioPinValue.High);
                    }

                }
                //else
                //   _logger.LogInformation("GpioService: PreparePins: Pin is already open (" + p.Name + ")");
            });
        }
        private void CreateController()
        {
            if (Controller != null)
                return;
            try
            {
                if (_hostingEnvironment.IsDevelopment())
                {
                    _logger.LogInformation("GpioService: Using mock GpioController...");
                    Controller = new MockGpioControllerWrapper();
                }
                else
                    Controller = new GpioControllerWrapper();
                _logger.LogInformation("GpioService: Initiated GpioController successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError("GpioService: Encountered an error\r\n");
                _logger.LogError(ex.StackTrace);
            }
        }
        public void Setup()
        {
            CleanUp();
            var config = _deviceConfiguration.LoadDeviceConfiguration();
            var sensors = config.Sensors;
            _logger.LogInformation($"GpioService setup {sensors.Count()} sensors found...");
            sensors.ToList().ForEach(s =>
            {
                _logger.LogInformation($"+- (ID: {s.Id}) {s.Name} Pin: {s.Pin} Polarity: {s.Polarity}");
                RegisterDevicePin(s);
            });
        }
        public void CleanUp()
        {
            Pins.ForEach(p =>
            {
                if (Controller.IsPinOpen(p.Pin))
                    Controller.ClosePin(p.Pin);
            });
            Pins.Clear();
        }
        public List<DeviceSensor> GetAllSensors()
        {
            return Pins;
            /* var inputPins = Pins.Where(p => p.Polarity == Polarity.Input).ToList();
            inputPins.ForEach(p => {
                var val = Controller.Read(p.Pin);
                _logger.LogInformation("Pin Value for " + p.Name + ": " + val);
                p.Value = val;
            });
            return inputPins;
            */
        }
        public GpioPinValue GetPinValue(DeviceSensor sensor)
        {
            var pin = Pins.First(p => p.Id == sensor.Id);
            //var val = Controller.Read(pin.Pin);
            //pin.Value = val;
            return pin.Value.Value;
        }
        public void RegisterDevicePin(DeviceSensor deviceSensor)
        {
            CreateController();
            Pins.Add(deviceSensor);
            try
            {
                PreparePins();
            }
            catch(Exception e)
            {
                Pins.Remove(deviceSensor);
                _logger.LogError($"Could not register device sensor ({deviceSensor.Name} Pin Number: {deviceSensor.Pin} Polarity: {deviceSensor.Polarity})");
                _logger.LogError(e.Message);
            }
        }
        public void SetPinValue(DeviceSensor sensor, GpioPinValue pinValue)
        {
            var pin = Pins.First(p => p.Id == sensor.Id);
            if(pin.AlwaysOn) //invert the value we are setting
                pinValue = pinValue == GpioPinValue.Low ? GpioPinValue.High : GpioPinValue.Low;
            pin.Value = pinValue;
            Controller.Write(pin.Pin, pinValue);
        }
        public void OnDeviceSensorTriggered(DeviceSensor p,object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {
            //p.Value = Controller.Read(p.Pin);

            p.Value = pinValueChangedEventArgs.ChangeType == PinEventTypes.Falling ? GpioPinValue.Low : GpioPinValue.High;
            p.OnSensorTriggered(sender, 0);
        }
        }
}
