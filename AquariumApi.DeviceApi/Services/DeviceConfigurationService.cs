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
        DeviceLoginResponse LoadAccountInformation();
        DeviceConfiguration LoadDeviceConfiguration();
        void SaveAccountInformation(DeviceLoginResponse loginResponse);
        void SaveDeviceConfiguration(DeviceConfiguration deviceConfiguration);
    }
    public class DeviceConfigurationService : IDeviceConfigurationService
    {
        private string _filePath;
        private string _accountFilePath;
        private IConfiguration _config;
        private ILogger<DeviceConfigurationService> _logger;
        private Action _bootstrapSetup;

        public Action _bootstrapCleanup { get; private set; }

        public DeviceConfigurationService(IConfiguration config, ILogger<DeviceConfigurationService> logger)
        {
            _config = config;
            _logger = logger;
            _filePath = config["configuration"];
            _accountFilePath = "login.json";
        }
        public void SaveAccountInformation(DeviceLoginResponse loginResponse)
        {
            var config = LoadDeviceConfiguration();
            if(loginResponse == null)
            {
                config.Aquarium = null;
                SaveDeviceConfiguration(config);
                File.Delete(_accountFilePath);
                return;
            }
            config.Aquarium = loginResponse.Aquarium;
            SaveDeviceConfiguration(config);
            File.WriteAllText(_accountFilePath, JsonConvert.SerializeObject(loginResponse));
        }
        public DeviceLoginResponse LoadAccountInformation()
        {
            DeviceLoginResponse accountInformation = null;
            if (File.Exists(_accountFilePath))
            {
                try
                {
                    accountInformation = JsonConvert.DeserializeObject<DeviceLoginResponse>(File.ReadAllText(_accountFilePath));

                }
                catch (Exception e)
                {
                    _logger.LogWarning($"Stored configuration is corrupted.");
                }
            }
            if (accountInformation == null)
            {
                _logger.LogInformation($"No account information found.");
                return null;
            }
            return accountInformation;
        }
        public void SaveDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(deviceConfiguration));
        }
        public DeviceConfiguration LoadDeviceConfiguration()
        {
            DeviceConfiguration deviceConfiguration = null;
            if (File.Exists(_filePath))
            {
                try
                {
                    deviceConfiguration = JsonConvert.DeserializeObject<DeviceConfiguration>(File.ReadAllText(_filePath));
                    return deviceConfiguration;
                }
                catch
                {
                    return null;
                }
            }
            else
                return null;
        }
        public void RegisterService(Action setupCallback, Action cleanUpCallback = null)
        {
            _bootstrapSetup += setupCallback;
            _bootstrapCleanup += cleanUpCallback;
        }
        public void Setup()
        {
            _bootstrapSetup();
        }
        public void CleanUp()
        {
            _bootstrapCleanup();
            _bootstrapSetup = null;
            _bootstrapCleanup = null;
        }
    }
}
