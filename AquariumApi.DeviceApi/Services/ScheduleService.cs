using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IScheduleService
    {
    }
    public class ScheduleService : IScheduleService
    {
        private IConfiguration _config;
        private ILogger<ScheduleService> _logger;
        private IDeviceService _deviceService;
        private AquariumClient _aquariumClient;

        public ScheduleService(IConfiguration config, ILogger<ScheduleService> logger,IDeviceService deviceService,AquariumClient aquariumClient)
        {
            _config = config;
            _logger = logger;
            _deviceService = deviceService;
            _aquariumClient = aquariumClient;

            var t = Task.Run(() =>
            {
                var ticks = 0;
                while(true)
                {
                    ticks++;
                    TakeSnapshot();
                    Thread.Sleep(15 * 60000);
                }
            });
        }

        private void TakeSnapshot()
        {
            _logger.LogWarning("Taking snapshot...");
            var device = _deviceService.GetDevice();
            var snapshot = _deviceService.TakeSnapshot();
            var photo = _deviceService.TakePhoto(device.CameraConfiguration);
            _aquariumClient.SendAquariumSnapshot(snapshot, photo);
        }
    }
}
