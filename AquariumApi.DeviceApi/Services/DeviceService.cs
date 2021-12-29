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
    public interface IDeviceService: IDeviceSetupService
    {
        AquariumSnapshot TakeSnapshot();
        byte[] TakePhoto(CameraConfiguration configuration);

        Task<AquariumDevice> PingAquariumService();
        AquariumSnapshot SendAquariumSnapshotToHost(AquariumSnapshot snapshot, byte[] photo);


        void PerformSnapshotTask(DeviceScheduleTask task);
    }
    public class DeviceService : IDeviceService
    {
        private IConfiguration _config;
        private ILogger<DeviceService> _logger;
        private IHardwareService _hardwareService;
        //private ScheduleService _scheduleService;
        private IAquariumClient _aquariumClient;
        private IAquariumAuthService _aquariumAuthService;

        public DeviceService(IConfiguration config, 
            ILogger<DeviceService> logger, 
            IHardwareService hardwareService, 
            IAquariumAuthService aquariumAuthService, 
            //ScheduleService scheduleService,
            IAquariumClient aquariumClient)
        {
            _config = config;
            _logger = logger;
            _hardwareService = hardwareService;
            //_scheduleService = scheduleService;
            _aquariumClient = aquariumClient;
            _aquariumAuthService = aquariumAuthService;
        }

        public byte[] TakePhoto(CameraConfiguration configuration)
        {
            return _hardwareService.TakePhoto(configuration);
        }

        public AquariumSnapshot TakeSnapshot()
        {
            var snapshot = new AquariumSnapshot()
            {
                StartTime = DateTime.UtcNow
            };
            //if (_accountLogin.Aquarium.Device.EnabledTemperature) snapshot.Temperature = _hardwareService.GetTemperatureC();
            return snapshot;
        }

        public async Task<AquariumDevice> PingAquariumService()
        {
            var response = await _aquariumClient.PingAquariumService();
            return response;
        }

        public AquariumSnapshot SendAquariumSnapshotToHost(AquariumSnapshot snapshot, byte[] photo)
        {
            return _aquariumClient.SendAquariumSnapshotToHost(snapshot, photo);
        }

        public void Setup()
        {
        }

        public void PerformSnapshotTask(DeviceScheduleTask task)
        {
            _logger.LogInformation("Taking aquarium snapshot...");
            var device = _aquariumAuthService.GetAquarium().Device;
            var cameraConfiguration = device.CameraConfiguration;
            var snapshot = TakeSnapshot();
            var photo = TakePhoto(cameraConfiguration);

            SendAquariumSnapshotToHost(snapshot, photo);
            _logger.LogInformation("Aquarium snapshot sent successfully");
        }

        public void CleanUp()
        {
            //dont need to do anything
        }
    }

}
