using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
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
        byte[] TakePhoto(int deviceId);
        bool SetAquarium(int deviceId, int aquariumId);
        string GetDeviceLog(int deviceId);
        DeviceInformation GetDeviceInformation(int deviceId);

        void ApplyScheduleAssignment(int deviceId,List<DeviceSchedule> deviceSchedules);
        void ClearDeviceLog(int deviceId);
        ScheduleState GetDeviceScheduleStatus(int deviceId);
        void PerformScheduleTask(int deviceId, DeviceScheduleTask deviceScheduleTask);
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
            snapshot.AquariumId = device.AquariumId;
            return snapshot;
        }
        public byte[] TakePhoto(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Snapshot/TakePhoto";

            _logger.LogInformation($"Taking photo on device...");
            using (var client2 = new HttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                var config = device.CameraConfiguration;
                //config.Device = null;
                var httpContent = new StringContent(JsonConvert.SerializeObject(config, jss), Encoding.UTF8, "application/json");
                var result = client2.PostAsync(path, httpContent).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not take photo");
                return result.Content.ReadAsByteArrayAsync().Result;
            }
        }
        public string GetDeviceLog(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Log";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(path).Result;
            if (response.IsSuccessStatusCode)
                return response.Content.ReadAsStringAsync().Result;
            throw new KeyNotFoundException();
        }
        public void ClearDeviceLog(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Log/Clear";
            HttpClient client = new HttpClient();


            var httpContent = new StringContent("", Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(path, httpContent).Result;
            if (!response.IsSuccessStatusCode)
                throw new KeyNotFoundException();
        }

        public DeviceInformation GetDeviceInformation(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Information";
            HttpClient client = new HttpClient();
            var data = client.GetStringAsync(path).Result;
            var d = JsonConvert.DeserializeObject<DeviceInformation>(data);
            return d;
        }

        public void ApplyScheduleAssignment(int deviceId, List<DeviceSchedule> deviceSchedules)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Schedule";
            using (var client2 = new HttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                var config = device.CameraConfiguration;
                //config.Device = null;

                var httpContent = new StringContent(JsonConvert.SerializeObject(deviceSchedules, jss), Encoding.UTF8, "application/json");
                var result = client2.PostAsync(path, httpContent).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not apply schedule assignment to device");
            }
        }

        public ScheduleState GetDeviceScheduleStatus(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Schedule/Status";
            HttpClient client = new HttpClient();
            var data = client.GetStringAsync(path).Result;
            var state = JsonConvert.DeserializeObject<ScheduleState>(data);
            return state;
        }

        public void PerformScheduleTask(int deviceId,DeviceScheduleTask deviceScheduleTask)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/Schedule";
            using (var client2 = new HttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                var config = device.CameraConfiguration;
                //config.Device = null;

                var httpContent = new StringContent(JsonConvert.SerializeObject(deviceScheduleTask, jss), Encoding.UTF8, "application/json");
                var result = client2.PostAsync(path, httpContent).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not perform task on device");
            }
        }
    }
}
