using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IDeviceConfigurationService
    {
        DeviceConfiguration LoadDeviceConfiguration();
        void SaveDeviceConfiguration(DeviceConfiguration deviceConfiguration);
    }
    public class DeviceConfigurationService : IDeviceConfigurationService
    {
        private string _filePath;
        private IConfiguration _config;
        private ILogger<DeviceConfigurationService> _logger;


        public DeviceConfigurationService(IConfiguration config, ILogger<DeviceConfigurationService> logger)
        {
            _config = config;
            _logger = logger;
            _filePath = config["configuration"];
        }
        public void SaveDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(deviceConfiguration));
        }
        public DeviceConfiguration LoadDeviceConfiguration()
        {
            DeviceConfiguration deviceConfiguration = null;
            if (File.Exists(_filePath)) 
                deviceConfiguration = JsonConvert.DeserializeObject<DeviceConfiguration>(File.ReadAllText(_filePath));
            if(deviceConfiguration == null)
            {
                _logger.LogInformation($"No device configuration found. Applying default configuration...");
                deviceConfiguration = new DeviceConfiguration();
                SaveDeviceConfiguration(deviceConfiguration);
            }
            return deviceConfiguration;
        }
    }
}
