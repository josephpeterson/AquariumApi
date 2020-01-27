using AquariumApi.DeviceApi.Clients;
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
        byte[] TakePhoto(CameraConfiguration configuration);

        void PingAquariumService();
        AquariumSnapshot SendAquariumSnapshotToHost(string host, AquariumSnapshot snapshot, byte[] photo);
        DeviceLoginResponse GetConnectionInformation();
    }
    public class DeviceService : IDeviceService
    {
        private IConfiguration _config;
        private ILogger<DeviceService> _logger;
        private DeviceLoginResponse _accountLogin;
        private IHardwareService _hardwareService;
        private IAquariumClient _aquariumClient;

        public DeviceService(IConfiguration config, ILogger<DeviceService> logger,IHardwareService hardwareService,IAquariumClient aquariumClient)
        {
            _config = config;
            _logger = logger;
            _hardwareService = hardwareService;
            _aquariumClient = aquariumClient;
        }

        public byte[] TakePhoto(CameraConfiguration configuration)
        {
            return _hardwareService.TakePhoto(configuration);
        }

        public AquariumSnapshot TakeSnapshot()
        {
            var snapshot = new AquariumSnapshot()
            {
                Date = DateTime.Now.ToUniversalTime()
            };
            if (_accountLogin.Aquarium.Device.EnabledTemperature) snapshot.Temperature = _hardwareService.GetTemperatureC();
            return snapshot;
        }

        public async void PingAquariumService()
        {
            var response = await _aquariumClient.ValidateAuthenticationToken();
            _accountLogin = response;
            _logger.LogInformation("Device information found for aquarium \"" + _accountLogin.Aquarium.Name + "\"");
        }
        public AquariumSnapshot SendAquariumSnapshotToHost(string host,AquariumSnapshot snapshot, byte[] photo)
        {
            return _aquariumClient.SendAquariumSnapshotToHost(host, snapshot, photo);
        }
        public DeviceLoginResponse GetConnectionInformation()
        {
            return _accountLogin;
        }
    }
}
