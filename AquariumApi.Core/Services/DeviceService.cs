﻿using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IDeviceService
    {
        AquariumDevice ScanHardware(int deviceId);
        bool Ping(int deviceId);
        AquariumSnapshot TakeSnapshot(int deviceId);
        AquariumPhoto TakePhoto(int deviceId,CameraConfiguration configuration);
        bool SetAquarium(int deviceId, int aquariumId);
    }
    public class DeviceService : IDeviceService
    {
        private readonly ILogger<DeviceService> _logger;
        private readonly IConfiguration _config;
        private readonly IAquariumDao _aquariumDao;

        public DeviceService(IConfiguration config,IAquariumDao aquariumDao, ILogger<DeviceService> logger)
        {
            _config = config;
            _aquariumDao = aquariumDao;
            _logger = logger;
        }

        public AquariumDevice ScanHardware(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Scan";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(path).Result;
            if (response.IsSuccessStatusCode)
                return response.Content.ReadAsAsync<AquariumDevice>().Result;
            throw new KeyNotFoundException();
        }
        public bool Ping(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Ping";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(path).Result;
            if (response.IsSuccessStatusCode)
                return true;
            return false;
        }
        public bool SetAquarium(int deviceId,int aquariumId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Ping";
            HttpClient client = new HttpClient();
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            var httpContent = new StringContent(JsonConvert.SerializeObject(device, jss), Encoding.UTF8, "application/json");
            var result = client.PostAsync(path, httpContent).Result;
            if (result.IsSuccessStatusCode)
                return true;
            return false;
        }

        public AquariumSnapshot TakeSnapshot(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Snapshot/Take";
            HttpClient client = new HttpClient();
            var data = client.GetStringAsync(path).Result;
            var snapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(data);
            snapshot.AquariumId = device.AquariumId.Value;
            return snapshot;
        }
        public AquariumPhoto TakePhoto(int deviceId,CameraConfiguration configuration)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var photos = _aquariumDao.GetAquariumPhotos(device.AquariumId.Value);
            var path = $"http://{device.Address}:{device.Port}/v1/Snapshot/TakePhoto";
            var downloadPath = String.Format(_config["PhotoFilePath"], device.AquariumId, DateTimeOffset.Now.ToUnixTimeMilliseconds());

            _logger.LogInformation($"Retrieving photo snapshot... [{downloadPath}]");
            using (var client2 = new HttpClient())
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(configuration), Encoding.UTF8, "application/json");
                var result = client2.PostAsync(path, httpContent).Result;
                Directory.CreateDirectory(Path.GetDirectoryName(downloadPath));
                using (Stream output = File.OpenWrite(downloadPath))
                    result.Content.ReadAsStreamAsync().Result.CopyTo(output);
            }
            if (!File.Exists(downloadPath))
                throw new KeyNotFoundException();
            _logger.LogInformation($"Snapshot photo was saved to location: {downloadPath}");
            var photo = new AquariumPhoto()
            {
                Date = new DateTime(),
                AquariumId = device.AquariumId.Value,
                Filepath = downloadPath
            };
            return photo;
        }
    }
}
