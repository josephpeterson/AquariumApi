using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
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
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IQueueService
    {
        List<string> GetAquariumSnapshotQueue();
        AquariumSnapshot GetQueuedAquariumSnapshot(string fileName);
        byte[] GetQueuedAquariumSnapshotPhoto(string fileName);
        int QueueAquariumSnapshot(AquariumSnapshot snapshot, byte[] photo);
    }
    public class QueueService : IQueueService
    {
        private IConfiguration _config;
        private ILogger<QueueService> _logger;
        private IHardwareService _hardwareService;
        private IAquariumClient _aquariumClient;

        public QueueService(IConfiguration config, ILogger<QueueService> logger,IHardwareService hardwareService,IAquariumClient aquariumClient)
        {
            _config = config;
            _logger = logger;
            _hardwareService = hardwareService;
            _aquariumClient = aquariumClient;
        }

        /* Store this snapshot for future uploading */
        public int QueueAquariumSnapshot(AquariumSnapshot snapshot, byte[] photo)
        {
            _logger.LogInformation("Queuing aquarium snapshot...");

            var json = JsonConvert.SerializeObject(snapshot);
            var fileCount = (from file in Directory.EnumerateFiles(_config["Queue:AquariumSnapshots"], "*.json", SearchOption.AllDirectories)
                             select file).Count();

            File.WriteAllText(_config["Queue:AquariumSnapshots"] + $"/${fileCount}.json", json);
            File.WriteAllBytes(_config["Queue:AquariumSnapshots"] + $"/${fileCount}.jpg", photo);
            return fileCount;
        }
        public List<string> GetAquariumSnapshotQueue()
        {
            var files = (from file in Directory.EnumerateFiles(_config["Queue:AquariumSnapshots"], "*.json", SearchOption.AllDirectories)
                         select file).Select(f => Path.GetFileName(f)).ToList();
            return files;
        }
        public AquariumSnapshot GetQueuedAquariumSnapshot(string fileName)
        {
            var json = File.ReadAllText(_config["Queue:AquariumSnapshots"] + $"/${fileName}.json");
            var snapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(json);
            return snapshot;
        }
        public byte[] GetQueuedAquariumSnapshotPhoto(string fileName)
        {
            var bytes = File.ReadAllBytes(_config["Queue:AquariumSnapshots"] + $"/${fileName}.jpg");
            return bytes;
        }
    }
}
