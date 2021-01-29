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
        void CleanUp();
        List<DeviceSensor> GetAllSensors();
        void PreparePins();
        void RegisterDevicePin(DeviceSensor deviceSensor);
        void SetPinValue(DeviceSensor pin, PinValue pinValue);
    }
    public class GpioService : IGpioService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HardwareService> _logger;
        private readonly ISerialService _serialService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private List<DeviceSensor> Pins;
        private GpioController Controller;
        public bool ATOSystemRunning { get; private set; }

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
            Pins.ForEach(p =>
            {
                try
                {
                    if (!Controller.IsPinOpen(p.Pin))
                    {
                        Controller.OpenPin(p.Pin, p.Polarity == 0 ? PinMode.InputPullUp : PinMode.Output);


                        if (p.Polarity == 0) // ?? why
                            Controller.RegisterCallbackForPinValueChangedEvent(p.Pin, PinEventTypes.Falling, (object sender, PinValueChangedEventArgs pinValueChangedEventArgs) =>
                            {
                                p.OnSensorTriggered(sender, 0);
                            });

                    }
                    else
                        _logger.LogInformation("GpioService: PreparePins: Pin is already open (" + p.Name + ")");
                }
                catch(Exception e)
                {
                    _logger.LogError($"Could not prepare pin (Pin Number: {p.Pin} Type: {p.Type}) ");
                    _logger.LogError(e.Message);
                }
            });
        }
        public void CleanUp()
        {
            Pins.ForEach(p =>
            {
                if (Controller.IsPinOpen(p.Pin))
                    Controller.ClosePin(p.Pin);
            });
        }
        public List<DeviceSensor> GetAllSensors()
        {
            var inputPins = Pins.Where(p => p.Polarity == Polarity.Input).ToList();
            inputPins.ForEach(p => {
                var val = Controller.Read(p.Pin);
                _logger.LogInformation("Pin Value for " + p.Name + ": " + val);
                p.Value = val.ToString();
            });
            return inputPins;
        }
        public void RegisterDevicePin(DeviceSensor deviceSensor)
        {
            Pins.Add(deviceSensor);
            PreparePins();

        }
        public void SetPinValue(DeviceSensor pin, PinValue pinValue)
        {
            Controller.Write(pin.Pin, pinValue);
        }
}
    
}
