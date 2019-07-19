using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace AquariumApi.DeviceApi
{
    public interface IHardwareService
    {
        int GetTemperatureC();
        AquariumDevice ScanHardware();
        byte[] TakePhoto(CameraConfiguration config);
    }
    public class HardwareService : IHardwareService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HardwareService> _logger;
        private readonly ISerialService _serialService;

        public HardwareService(IConfiguration config, ILogger<HardwareService> logger,ISerialService serialService)
        {
            _config = config;
            _logger = logger;
            _serialService = serialService;
        }
        public AquariumDevice ScanHardware()
        {
            return new AquariumDevice()
            {
                EnabledTemperature = _serialService.CanRetrieveTemperature(),
                EnabledNitrate = _serialService.CanRetrieveNitrate(),
                EnabledPh = _serialService.CanRetrievePh(),
                EnabledPhoto = CanTakePhoto(),
            };
        }



        public bool CanTakePhoto()
        {
            try
            {
                File.Delete("hardwaretest.jpg");
                $"/usr/bin/raspistill -o hardwaretest.jpg".Bash();
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
            //Path.GetDirectoryName(config.Output);
            if (File.Exists(config.Output))
                File.Delete(config.Output);
            _logger.LogInformation($"Taking Photo: " + config);
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


    }
}
