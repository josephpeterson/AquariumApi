using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace AquariumApi.DeviceApi
{
    public interface IHardwareService
    {
        int GetTemperatureC();
        AquariumDevice ScanHardware();
        byte[] TakePhoto(CameraConfiguration config);
        void WaterChangeBeginDrain();
    }
    public class HardwareService : IHardwareService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HardwareService> _logger;
        private readonly ISerialService _serialService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HardwareService(IConfiguration config, ILogger<HardwareService> logger,ISerialService serialService,IHostingEnvironment hostingEnvironment)
        {
            _config = config;
            _logger = logger;
            _serialService = serialService;
            _hostingEnvironment = hostingEnvironment;
        }
        public AquariumDevice ScanHardware()
        {
            var port = _hostingEnvironment;

            var hardware = new AquariumDevice()
            {
                Port = _config["Port"],
                EnabledPhoto = CanTakePhoto(),
                EnabledTemperature = _serialService.CanRetrieveTemperature(),
                EnabledNitrate = _serialService.CanRetrieveNitrate(),
                EnabledPh = _serialService.CanRetrievePh(),
            };
            return hardware;
        }



        public bool CanTakePhoto()
        {
            try
            {
                File.Delete("hardwaretest.jpg");
                $"/usr/bin/raspistill -w 100 -h 100 -o hardwaretest.jpg".Bash();
                if(File.Exists("hardwaretest.jpg"))
                {
                    File.Delete("hardwaretest.jpg");
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public byte[] TakePhoto(CameraConfiguration config)
        {
            config.Output = "temp.jpg";
            if (_hostingEnvironment.IsDevelopment())
                return System.IO.File.ReadAllBytes(config.Output);
            //Path.GetDirectoryName(config.Output);
            if (File.Exists(config.Output))
                File.Delete(config.Output);
            _logger.LogInformation($"Taking Photo...");
            $"/usr/bin/raspistill {config}".Bash();
            return System.IO.File.ReadAllBytes(config.Output);
        }
        public int GetTemperatureC()
        {
            return _serialService.GetTemperatureC();
        }
        public bool CanRetrievePh()
        {
            return false;
        }
        public bool CanRetrieveNitrate()
        {
            return false;
        }



        public List<DeviceSensor> ReadAllSensors()
        {
            List<DeviceSensor> sensors = new List<DeviceSensor>();
            sensors.Add(
                new DeviceSensor()
                {
                    Pin = 40,
                    Type = SensorTypes.FloatSwitch,
                    Polarity = Polarity.Input
                }
            );

            sensors.ForEach(s =>
            {
               
            });

            return sensors;
        }


        public void WaterChangePrepare()
        {
            //Turn off return pump and Protein Skimmer

        }
        public void WaterChangeBeginDrain()
        {
            //Open solenoid until float sensor detects low water level

            /*
            var solenoid = Pi.Gpio[21];
            solenoid.PinMode = GpioPinDriveMode.Output;
            _logger.LogInformation("[WaterChange] SOLENOID OPEN");
            solenoid.Write(GpioPinValue.High);
            System.Threading.Thread.Sleep(2000);
            solenoid.Write(GpioPinValue.Low);
            _logger.LogInformation("[WaterChange] SOLENOID CLOSED");
            */
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
