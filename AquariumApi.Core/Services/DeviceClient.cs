using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Hosting;
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
    public interface IDeviceClient
    {
        AquariumDevice ScanHardware();
        bool Ping();
        string GetDeviceLog();
        void ClearDeviceLog();
        DeviceInformation GetDeviceInformation();


        AquariumSnapshot TakeSnapshot(int deviceId);
        byte[] TakePhoto(int deviceId);

        void ApplyScheduleAssignment(int deviceId,List<DeviceSchedule> deviceSchedules);
        ScheduleState GetDeviceScheduleStatus();
        void PerformScheduleTask(DeviceScheduleTask deviceScheduleTask);
        void ApplyUpdatedDevice(AquariumDevice aquariumDevice);


        ATOStatus GetDeviceATOStatus(); //correct
        ATOStatus PerformDeviceATO(int maxRuntime);
        ATOStatus StopDeviceATO();
        void Configure(AquariumDevice device);
    }
    public class DeviceClient : IDeviceClient
    {
        private readonly ILogger<DeviceClient> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _config;
        private readonly IAquariumDao _aquariumDao;
        private AquariumDevice Device;

        private Dictionary<string, string> HostOveride = new Dictionary<string, string>();

        public DeviceClient(IConfiguration config,IHostingEnvironment hostingEnvironment,IAquariumDao aquariumDao, ILogger<DeviceClient> logger)
        {
            _config = config;
            _aquariumDao = aquariumDao;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;

            _config.GetSection("HostOverride").Bind(HostOveride);
        }

        
        public void Configure(AquariumDevice device)
        {
            Device = device;
        }
        private HttpClient GetHttpClient()
        {
            //var token = _aquariumAuthService.GetToken();
            //if (token == null)
            //    throw new Exception("No authentication token available");
            HttpClient client = new HttpClient();

            //check for host override
            var overrideHost = HostOveride.Keys.Where(k => k == Device.Address).FirstOrDefault();
            if(_hostingEnvironment.IsDevelopment() && overrideHost != null)
            {
                var newHost = HostOveride[Device.Address];
                _logger.LogInformation($"Overriding host name {Device.Address}:{Device.Port} with {newHost}");
                client.BaseAddress = new Uri($"http://{newHost}:{Device.Port}");
            }
            else
                client.BaseAddress = new Uri($"http://{Device.Address}:{Device.Port}");
            client.Timeout = TimeSpan.FromSeconds(30);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            return client;
        }



        public AquariumDevice ScanHardware()
        {
            var path = $"/v1/Scan";
            using (var client = GetHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsAsync<AquariumDevice>().Result;
                throw new KeyNotFoundException();
            }
        }
        public bool Ping()
        {
            var path = $"/v1/Ping";
            using (var client = GetHttpClient())
            {
                    HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                    return true;
                return false;
            }
        }
        public string GetDeviceLog()
        {
            var path = $"/v1/Log";
            using (var client = GetHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                    return response.Content.ReadAsStringAsync().Result;
                throw new KeyNotFoundException();

            }
        }
        public void ClearDeviceLog()
        {
            var path = "/v1/Log/Clear";
            using (var client = GetHttpClient())
            {
                var httpContent = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(path, httpContent).Result;
                if (!response.IsSuccessStatusCode)
                    throw new KeyNotFoundException();
            }
        }

        public DeviceInformation GetDeviceInformation()
        {
            var path = "/v1/Information";
            using (var client = GetHttpClient())
            {
                var data = client.GetStringAsync(path).Result;
                var d = JsonConvert.DeserializeObject<DeviceInformation>(data);
                return d;
            }
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
        public void ApplyUpdatedDevice(AquariumDevice aquariumDevice)
        {
            var path = $"/v1/Device";

            using (var client2 = GetHttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                var httpContent = new StringContent(JsonConvert.SerializeObject(aquariumDevice, jss), Encoding.UTF8, "application/json");
                var result = client2.PostAsync(path, httpContent).Result;
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"{result.StatusCode}: {result.ReasonPhrase}");
                    _logger.LogError($"Path: {path}");
                    throw new Exception("Could not send updated device information to device");
                }
            }
            _logger.LogInformation("Device information updated on device");
        }

        public ScheduleState GetDeviceScheduleStatus()
        {
            var path = "/v1/Schedule";
            using (var client = GetHttpClient())
            {
                var data = client.GetStringAsync(path).Result;
                var state = JsonConvert.DeserializeObject<ScheduleState>(data);
                return state;
            }
        }

        public void PerformScheduleTask(DeviceScheduleTask deviceScheduleTask)
        {
            var path = "/v1/Schedule/Perform";
            using (var client = GetHttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                var config = Device.CameraConfiguration;
                //config.Device = null;

                var httpContent = new StringContent(JsonConvert.SerializeObject(deviceScheduleTask, jss), Encoding.UTF8, "application/json");
                var result = client.PostAsync(path, httpContent).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not perform task on device");
            }
        }


        /* Get device ATO status */
        public ATOStatus GetDeviceATOStatus()
        {
            var path = "/v1/WaterChange/ATO/Status";
            var client = GetHttpClient();
            var data = client.GetStringAsync(path).Result;
            var state = JsonConvert.DeserializeObject<ATOStatus>(data);
            state.DeviceId = Device.Id; //the device could send bad data
            return state;
        }
        
        public ATOStatus PerformDeviceATO(int maxRuntime)
        {
            var path = "/v1/WaterChange/ATO";
            using (var client = GetHttpClient())
            {
                var httpContent = new StringContent(maxRuntime.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(path, httpContent).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not perform ATO on device");

                var atoStatus = JsonConvert.DeserializeObject<ATOStatus>(result.Content.ReadAsStringAsync().Result);
                atoStatus = _aquariumDao.UpdateATOStatus(atoStatus);
                return atoStatus;
            }
        }
        public ATOStatus StopDeviceATO()
        {
            var path = $"/v1/WaterChange/ATO/Stop";
            using (var client = GetHttpClient())
            {
                var result = client.PostAsync(path, null).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not stop ATO on device");

                var atoStatus = JsonConvert.DeserializeObject<ATOStatus>(result.Content.ReadAsStringAsync().Result);
                atoStatus = _aquariumDao.UpdateATOStatus(atoStatus);
                return atoStatus;
            }
        }
        }
}
