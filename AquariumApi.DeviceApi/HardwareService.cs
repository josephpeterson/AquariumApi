using AquariumApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MMALSharp;
using MMALSharp.Handlers;
using MMALSharp.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IHardwareService
    {
        AquariumDevice ScanHardware();
    }
    public class HardwareService : IHardwareService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HardwareService> _logger;

        public HardwareService(IConfiguration config, ILogger<HardwareService> logger)
        {
            _config = config;
            _logger = logger;
        }
        public AquariumDevice ScanHardware()
        {
            return new AquariumDevice()
            {
                EnabledPhoto = CanTakePhoto(),
                EnabledTemperature = CanRetrieveTemperature(),
                EnabledNitrate = CanRetrieveNitrate(),
                EnabledPh = CanRetrievePh()
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
        public bool CanRetrieveTemperature()
        {
            return false;
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
