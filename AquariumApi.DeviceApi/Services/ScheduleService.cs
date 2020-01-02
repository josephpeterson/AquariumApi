using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IScheduleService
    {
        void SaveScheduleAssignment(List<DeviceSchedule> deviceSchedules);
        void Start();
    }
    public class ScheduleService : IScheduleService
    {
        private IConfiguration _config;
        private ILogger<ScheduleService> _logger;
        private IDeviceService _deviceService;
        private IQueueService _queueService;

        public Task thread;

        public ScheduleService(IConfiguration config, ILogger<ScheduleService> logger,IDeviceService deviceService,IQueueService queueService)
        {
            _config = config;
            _logger = logger;
            _deviceService = deviceService;
            _queueService = queueService;
        }

        public void Start()
        {
            if (thread != null && !thread.IsCanceled)
                thread.Dispose();

            var deviceSchedules = LoadAllSchedules();

            _logger.LogInformation($"{deviceSchedules.Count} schedules have been loaded");

            //Get schedule from api... start it... allow api to stop and set schedules

            _logger.LogWarning("Starting schedule...");
            thread = Task.Run(() =>
            {
                var ticks = 0;
                while (true)
                {
                    ticks++;
                    _deviceService.CheckAvailableHardware();
                    TakeSnapshot();
                    Thread.Sleep(15 * 60000);
                }
            });
        }

        private void TakeSnapshot()
        {
                _logger.LogWarning("Taking snapshot...");
                var device = _deviceService.GetDevice();
            try
            {
                var snapshot = _deviceService.TakeSnapshot();
                var photo = _deviceService.TakePhoto(device.CameraConfiguration);

                try
                {
                    _deviceService.SendAquariumSnapshot(snapshot, photo);
                }
                catch(Exception e)
                {
                    //Queue snapshot
                    _queueService.QueueAquariumSnapshot(snapshot, photo);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"TakeSnapshot: { ex.Message } Details: { ex.ToString() }");
            }
        }


        public void SaveScheduleAssignment(List<DeviceSchedule> deviceSchedules)
        {
            var filepath = _config["ScheduleAssignmentPath"];
            var json = JsonConvert.SerializeObject(deviceSchedules);
            System.IO.File.WriteAllText(filepath, json);
        }
        public List<DeviceSchedule> LoadAllSchedules()
        {
            var filepath = _config["ScheduleAssignmentPath"];
            var json = System.IO.File.ReadAllText(filepath);
            return JsonConvert.DeserializeObject<List<DeviceSchedule>>(json);
        }
    }
}
