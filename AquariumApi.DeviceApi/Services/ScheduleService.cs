﻿using AquariumApi.DeviceApi.Clients;
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
        void Start();
    }
    public class ScheduleService : IScheduleService
    {
        private IConfiguration _config;
        private ILogger<ScheduleService> _logger;
        private IDeviceService _deviceService;

        public ScheduleService(IConfiguration config, ILogger<ScheduleService> logger,IDeviceService deviceService)
        {
            _config = config;
            _logger = logger;
            _deviceService = deviceService;
        }

        public void Start()
        {
            //Get schedule from api... start it... allow api to stop and set schedules

            _logger.LogWarning("Starting schedule...");
            var t = Task.Run(() =>
            {
                var ticks = 0;
                while (true)
                {
                    ticks++;
                    TakeSnapshot();
                    Thread.Sleep(15 * 60000);
                }
            });
        }

        private void TakeSnapshot()
        {
            try
            {
                _logger.LogWarning("Taking snapshot...");
                var device = _deviceService.GetDevice();
                var snapshot = _deviceService.TakeSnapshot();
                var photo = _deviceService.TakePhoto(device.CameraConfiguration);
                _deviceService.SendAquariumSnapshot(snapshot, photo);
            }
            catch(Exception ex)
            {
                _logger.LogError($"TakeSnapshot: { ex.Message } Details: { ex.ToString() }");
            }
        }
    }
}
