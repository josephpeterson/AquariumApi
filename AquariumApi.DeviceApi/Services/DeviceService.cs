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
        byte[] TakePhoto(CameraConfiguration configuration);

        Task<AquariumDevice> PingAquariumService();
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
        public async Task<AquariumDevice> PingAquariumService()
        {
            var response = await _aquariumClient.PingAquariumService();
            return response;
        }
    }
}
