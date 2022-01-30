﻿using AquariumApi.DataAccess;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
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
        string GetDeviceLog();
        void ClearDeviceLog();
        DeviceInformation PingDevice();


        AquariumSnapshot TakeSnapshot(int deviceId);
        byte[] TakePhoto(int deviceId);

        ScheduleState GetDeviceScheduleStatus();
        ScheduledJob PerformScheduleTask(ScheduledJob scheduledJob);
        void ApplyUpdatedDevice(AquariumDevice aquariumDevice);
        void ApplyAssignedAquarium(Aquarium assignedAquarium);


        void Configure(AquariumDevice device);
        Task<DeviceSensorTestRequest> TestDeviceSensor(DeviceSensorTestRequest testRequest);
        ScheduledJob StopScheduledJob(ScheduledJob scheduledJob);
        List<ScheduledJob> GetAllScheduledJobs();
        void RenewDevice();
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
            if (Device == null)
                throw new Exception("Aquarium Device not configured for DeviceClient");

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
        private void ValidateResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errContent =  response.Content.ReadAsStringAsync().Result;
                DeviceException err = null;
                try
                {
                    err = JsonConvert.DeserializeObject<DeviceException>(errContent);
                }
                finally
                {
                    if (err == null)
                        throw new AquariumServiceException("Unknown error response received from device");
                    throw err;
                }
                
            }
        }


        public string GetDeviceLog()
        {
            var path = DeviceOutboundEndpoints.LOG;
            using (var client = GetHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(path).Result;
                ValidateResponse(response);
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        public void ClearDeviceLog()
        {
            var path = DeviceOutboundEndpoints.LOG_CLEAR;
            using (var client = GetHttpClient())
            {
                var httpContent = new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(path, httpContent).Result;
                ValidateResponse(response);
            }
        }
        /// <summary>
        /// This is the most basic request to the Device
        /// </summary>
        /// <returns></returns>
        public DeviceInformation PingDevice()
        {
            var path = DeviceOutboundEndpoints.PING;
            using (var client = GetHttpClient())
            {
                var response = client.GetAsync(path).Result;
                ValidateResponse(response);
                var d = JsonConvert.DeserializeObject<DeviceInformation>(response.Content.ReadAsStringAsync().Result);
                return d;
            }
        }
        /// <summary>
        /// Tell the device to renew it's authentication token with us
        /// </summary>
        /// <returns></returns>
        public void RenewDevice()
        {
            var path = DeviceOutboundEndpoints.AUTH_RENEW;
            using (var client = GetHttpClient())
            {
                var response = client.GetAsync(path).Result;
                ValidateResponse(response);
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
        
        /// <summary>
        /// Send the device the latest information we have. This will ultimately resort in the device rebooting services
        /// </summary>
        /// <param name="aquariumDevice"></param>
        public void ApplyUpdatedDevice(AquariumDevice aquariumDevice)
        {
            var path = DeviceOutboundEndpoints.UPDATE;
            using (var client2 = GetHttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                var httpContent = new StringContent(JsonConvert.SerializeObject(aquariumDevice, jss), Encoding.UTF8, "application/json");
                var result = client2.PostAsync(path, httpContent).Result;
                ValidateResponse(result);
            }
            _logger.LogInformation("Device information updated on device");
        }
        public void ApplyAssignedAquarium(Aquarium assignedAquarium)
        {
            var path = DeviceOutboundEndpoints.UPDATE;
            using (var client2 = GetHttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                var httpContent = new StringContent(JsonConvert.SerializeObject(assignedAquarium, jss), Encoding.UTF8, "application/json");
                var result = client2.PostAsync(path, httpContent).Result;
                ValidateResponse(result);
            }
            _logger.LogInformation("Assigned aquarium deployed to device.");
        }

        public ScheduleState GetDeviceScheduleStatus()
        {
            var path = DeviceOutboundEndpoints.SCHEDULE_STATUS;
            using (var client = GetHttpClient())
            {
                var result = client.GetAsync(path).Result;
                ValidateResponse(result);
                var state = JsonConvert.DeserializeObject<ScheduleState>(result.Content.ReadAsStringAsync().Result);
                return state;
            }
        }

        public ScheduledJob PerformScheduleTask(ScheduledJob scheduledJob)
        {
            var path = DeviceOutboundEndpoints.SCHEDULE_TASK_PERFORM;
            using (var client = GetHttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                var config = Device.CameraConfiguration;
                //config.Device = null;

                var httpContent = new StringContent(JsonConvert.SerializeObject(scheduledJob, jss), Encoding.UTF8, "application/json");
                var result = client.PostAsync(path, httpContent).Result;
                ValidateResponse(result);
                var runningJob = JsonConvert.DeserializeObject<ScheduledJob>(result.Content.ReadAsStringAsync().Result);
                return runningJob;
            }
        }
        public ScheduledJob StopScheduledJob(ScheduledJob scheduledJob)
        {
            var path = DeviceOutboundEndpoints.SCHEDULE_SCHEDULEDJOB_STOP;
            using (var client = GetHttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                var httpContent = new StringContent(JsonConvert.SerializeObject(scheduledJob, jss), Encoding.UTF8, "application/json");
                var result = client.PostAsync(path, httpContent).Result;
                ValidateResponse(result);
                var stoppedJob = JsonConvert.DeserializeObject<ScheduledJob>(result.Content.ReadAsStringAsync().Result);
                return stoppedJob;
            }
        }
        public List<ScheduledJob> GetAllScheduledJobs()
        {
            var path = DeviceOutboundEndpoints.SCHEDULE_REMAINING_TASKS;
            using (var client = GetHttpClient())
            {
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                var result = client.GetAsync(path).Result;
                ValidateResponse(result);
                var scheduledJobs = JsonConvert.DeserializeObject<List<ScheduledJob>>(result.Content.ReadAsStringAsync().Result);
                return scheduledJobs;
            }
        }


        /* Get device ATO status */
        #region Device ATO        
        #endregion

        #region Device Sensors
        public async Task<DeviceSensorTestRequest> TestDeviceSensor(DeviceSensorTestRequest testRequest)
        {
            var path = DeviceOutboundEndpoints.DEVICE_SENSOR_TEST;
            using (var client = GetHttpClient())
            {
                var content = JsonConvert.SerializeObject(testRequest);
                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                HttpResponseMessage result;
                try
                {
                    result = await client.PostAsync(path, httpContent);
                }
                catch(Exception ex)
                {
                    throw new AquariumServiceException("The target device was unavailable.");
                }
                ValidateResponse(result);

                var data = await result.Content.ReadAsStringAsync();
                var createdTestRequest = JsonConvert.DeserializeObject<DeviceSensorTestRequest>(data);
                return createdTestRequest;
            }
        }
        #endregion

        
    }
}
