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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IDeviceService
    {
        AquariumSnapshot TakeSnapshot();
        byte[] TakePhoto(CameraConfiguration configuration);

        Task<DeviceLoginResponse> PingAquariumService();
        AquariumSnapshot SendAquariumSnapshotToHost(AquariumSnapshot snapshot, byte[] photo);
        DeviceLoginResponse GetConnectionInformation();
        Task<bool> RenewAuthenticationToken();
        void ApplyCameraConfiguration(CameraConfiguration cameraConfiguration);
    }
    public class DeviceService : IDeviceService
    {
        private IConfiguration _config;
        private ILogger<DeviceService> _logger;
        private DeviceLoginResponse _accountLogin;
        private IHardwareService _hardwareService;
        private IAquariumClient _aquariumClient;

        public DeviceService(IConfiguration config, ILogger<DeviceService> logger, IHardwareService hardwareService, IAquariumClient aquariumClient)
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

        public async Task<DeviceLoginResponse> PingAquariumService()
        {
            try
            {
                var response = await _aquariumClient.ValidateAuthenticationToken();
                _accountLogin = response;
                _logger.LogInformation("Device information found for aquarium \"" + _accountLogin.Aquarium.Name + "\"");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not get device information from AquariumService: { ex.Message } Details: { ex.ToString() }");
                return null;
            }
        }
        public async Task<bool> RenewAuthenticationToken()
        {
            try
            {
                var response = await _aquariumClient.RenewAuthenticationToken();
                _accountLogin = response;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not renew authentication token from AquariumService: { ex.Message } Details: { ex.ToString() }");
                return false;
            }
        }
        public AquariumSnapshot SendAquariumSnapshotToHost(AquariumSnapshot snapshot, byte[] photo)
        {
            return _aquariumClient.SendAquariumSnapshotToHost(snapshot, photo);
        }
        public DeviceLoginResponse GetConnectionInformation()
        {
            return _accountLogin;
        }

        public void ApplyCameraConfiguration(CameraConfiguration cameraConfiguration)
        {
            _accountLogin.Aquarium.Device.CameraConfiguration = cameraConfiguration;
        }
    }
}
