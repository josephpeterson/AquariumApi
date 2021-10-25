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
    public interface IGpioService: IDeviceSetupService
    {
        List<DeviceSensor> GetAllSensors();
        GpioPinValue GetPinValue(DeviceSensor pin);
        void RegisterDevicePin(DeviceSensor deviceSensor);
        void SetPinValue(DeviceSensor pin, PinValue pinValue);
        DeviceSensorTestRequest TestDeviceSensor(DeviceSensorTestRequest testRequest);
    }
    public class GpioService : IGpioService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<GpioService> _logger;
        private readonly ISerialService _serialService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IAquariumAuthService _aquariumAuthService;
        private List<DeviceSensor> Pins = new List<DeviceSensor>();
        private IGpioControllerWrapper Controller;
        public bool ATOSystemRunning { get; private set; }

        public GpioService(IConfiguration config, ILogger<GpioService> logger, IAquariumAuthService aquariumAuthService,ISerialService serialService,IHostingEnvironment hostingEnvironment)
        {
            _config = config;
            _logger = logger;
            _serialService = serialService;
            _hostingEnvironment = hostingEnvironment;
            _aquariumAuthService = aquariumAuthService;
            CreateController();
        }
        private void PreparePins()
        {
            Pins.ForEach(p =>
            {
                if (!Controller.IsPinOpen(p.Pin))
                {
                    Controller.OpenPin(p.Pin, p.Polarity == 0 ? PinMode.InputPullUp : PinMode.Output);


                    if (p.Polarity == 0) // Input only
                        Controller.RegisterCallbackForPinValueChangedEvent(p.Pin, PinEventTypes.Falling, (object sender, PinValueChangedEventArgs pinValueChangedEventArgs) =>
                        {
                            p.OnSensorTriggered(sender, 0);
                        });

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
            var device = _aquariumAuthService.GetAquarium().Device;
            var sensors = device.Sensors;
            _logger.LogInformation($"{sensors.Count()} sensors found...");
            sensors.ToList().ForEach(s =>
            {
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
        public GpioPinValue GetPinValue(DeviceSensor pin)
        {
            var val = Controller.Read(pin.Pin);
            pin.Value = val;
            return val;
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
        public void SetPinValue(DeviceSensor pin, PinValue pinValue)
        {
            Controller.Write(pin.Pin, pinValue);
        }
        public DeviceSensorTestRequest TestDeviceSensor(DeviceSensorTestRequest testRequest)
        {
            var sensors = GetAllSensors();
            var sensor = sensors.Where(s => s.Id == testRequest.SensorId).FirstOrDefault();
            if (sensor == null) throw new DeviceException("Could not locate sensor on this device by that sensor id.");

            var maxTestRuntime = Convert.ToInt32(_config["WaterChangeMaximumRuntime"]);
            if (testRequest.Runtime > maxTestRuntime) throw new DeviceException($"Runtime exceeds maximum runtime allowed (Maximum allowed: {maxTestRuntime})");
            maxTestRuntime = testRequest.Runtime;


            _logger.LogWarning($"Testing sensor ({sensor.Name} Pin Number: {sensor.Pin} Polarity: {sensor.Polarity})");
            testRequest.StartTime = DateTime.UtcNow;
            SetPinValue(sensor, PinValue.High);
            Thread.Sleep(TimeSpan.FromSeconds(maxTestRuntime));
            SetPinValue(sensor, PinValue.Low);
            testRequest.EndTime = DateTime.UtcNow;

            var value = GetPinValue(sensor);
            _logger.LogWarning($"Testing sensor completed ({sensor.Name} Pin Number: {sensor.Pin} Polarity: {sensor.Polarity} Value: {value})");
            return testRequest;
        }
    }
    
}
