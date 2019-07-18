﻿using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IDeviceService
    {
        AquariumSnapshot TakeSnapshot();
        FileStreamResult TakePhoto(CameraConfiguration configuration);

        void PingAquariumService();
        void CheckAvailableHardware();
        AquariumDevice GetDevice();
        void SetDevice(AquariumDevice device);
    }
    public class DeviceService : IDeviceService
    {
        private IConfiguration _config;
        private ILogger<DeviceService> _logger;
        private AquariumDevice Device;
        private IHardwareService _hardwareService;
        private IAquariumClient _aquariumClient;

        public DeviceService(IConfiguration config, ILogger<DeviceService> logger,IHardwareService hardwareService,IAquariumClient aquariumClient)
        {
            _config = config;
            _logger = logger;
            _hardwareService = hardwareService;
            _aquariumClient = aquariumClient;
        }

        public void SetDevice(AquariumDevice device)
        {
            Device = device;
        }
        public AquariumDevice GetDevice()
        {
            return Device;
        }


        public FileStreamResult TakePhoto(CameraConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public AquariumSnapshot TakeSnapshot()
        {
            var snapshot = new AquariumSnapshot()
            {
                Date = DateTime.Now
            };
            if (Device.EnabledTemperature) snapshot.Temperature = _hardwareService.GetTemperatureC();
            return snapshot;
        }

        public void PingAquariumService()
        {
            Device = new AquariumDevice()
            {
                PrivateKey = _config["DeviceKey"]
            };
            CheckAvailableHardware();
            var actualDevice = _aquariumClient.GetDeviceInformation(Device);
            _logger.LogInformation("Device information found for aquarium \"" + actualDevice.Aquarium.Name + "\"");
            SetDevice(actualDevice);
        }

        public void CheckAvailableHardware()
        {
            if (Device == null)
                throw new Exception("No device information found from service.");
            var thisDevice = _hardwareService.ScanHardware();
            Device.EnabledLighting = thisDevice.EnabledLighting;
            Device.EnabledNitrate = thisDevice.EnabledNitrate;
            Device.EnabledNitrite = thisDevice.EnabledNitrite;
            Device.EnabledPh = thisDevice.EnabledPh;
            Device.EnabledPhoto = thisDevice.EnabledPhoto;
            Device.EnabledTemperature = thisDevice.EnabledTemperature;
        }
    }
}
