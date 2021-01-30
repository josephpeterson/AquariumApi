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
        string GetDeviceLog(int deviceId);
        DeviceInformation GetDeviceInformation(int deviceId);

        void ApplyScheduleAssignment(int deviceId,List<DeviceSchedule> deviceSchedules);
        void ClearDeviceLog(int deviceId);
        ScheduleState GetDeviceScheduleStatus(int deviceId);
        void PerformScheduleTask(int deviceId, DeviceScheduleTask deviceScheduleTask);
        void ApplyUpdatedDevice(AquariumDevice aquariumDevice);
        ATOStatus GetDeviceATOStatus(int deviceId);
        ICollection<DeviceSensor> GetDeviceSensors(int deviceId);
        DeviceSensor CreateDeviceSensor(int deviceId, DeviceSensor deviceSensor);
        void DeleteDeviceSensor(int deviceId, int deviceSensorId);
        ATOStatus PerformDeviceATO(int deviceId, int maxRuntime);
        ATOStatus StopDeviceATO(int deviceId);
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
        public void ApplyUpdatedDevice(AquariumDevice aquariumDevice)
        {
            var path = $"http://{aquariumDevice.Address}:{aquariumDevice.Port}/v1/Device";

            using (var client2 = new HttpClient())
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


        /* Get device ATO status */
        public ATOStatus GetDeviceATOStatus(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);

            try
            {
                var path = $"http://{device.Address}:{device.Port}/v1/WaterChange/ATO/Status";
                HttpClient client = new HttpClient();
                var data = client.GetStringAsync(path).Result;
                var state = JsonConvert.DeserializeObject<ATOStatus>(data);

                bool update = false;
                if (!state.Id.HasValue)
                {
                    //retrieve the latest status
                    var oldState = _aquariumDao.GetATOStatus(deviceId).Where(s => !s.Completed)
                        .OrderBy(s => s.UpdatedAt)
                        .FirstOrDefault();
                    if (oldState != null)
                        state.Id = oldState.Id;
                }
                //insert into db
                state = _aquariumDao.UpdateATOStatus(state);


                //This is a new insert in db, backfill the Id to the device
                if(update)
                {
                    //maybe we dont need to do this? todo
                    var httpContent = new StringContent(JsonConvert.SerializeObject(state), Encoding.UTF8, "application/json");
                    client.PutAsync(path,httpContent);
                }
                return state;
            }
            catch(Exception e)
            {
                _logger.LogInformation("Could not retrieve ATO status from device. Loading form cache...");
                //load from cache
                return _aquariumDao.GetATOStatus(deviceId).FirstOrDefault();
            }
        }
        public ATOStatus PerformDeviceATO(int deviceId, int maxRuntime)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/WaterChange/ATO";
            using (var client2 = new HttpClient())
            {
                var httpContent = new StringContent(maxRuntime.ToString(), Encoding.UTF8, "application/json");
                var result = client2.PostAsync(path, httpContent).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not perform ATO on device");

                var atoStatus = JsonConvert.DeserializeObject<ATOStatus>(result.Content.ReadAsStringAsync().Result);
                atoStatus = _aquariumDao.UpdateATOStatus(atoStatus);
                return atoStatus;
            }
        }
        public ATOStatus StopDeviceATO(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            var path = $"http://{device.Address}:{device.Port}/v1/WaterChange/ATO/Stop";
            using (var client2 = new HttpClient())
            {
                var result = client2.PostAsync(path, null).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not stop ATO on device");

                var atoStatus = JsonConvert.DeserializeObject<ATOStatus>(result.Content.ReadAsStringAsync().Result);
                atoStatus = _aquariumDao.UpdateATOStatus(atoStatus);
                return atoStatus;
            }
        }


        /* Device Sensors */
        public DeviceSensor CreateDeviceSensor(int deviceId,DeviceSensor deviceSensor)
        {
            deviceSensor.DeviceId = deviceId;
            deviceSensor = _aquariumDao.AddDeviceSensor(deviceSensor);
            /* todo tell rpi that sensors updated */
            return deviceSensor;
        }
        public ICollection<DeviceSensor> GetDeviceSensors(int deviceId)
        {
            var deviceSensors = _aquariumDao.GetDeviceSensors(deviceId);
            return deviceSensors;
        }
        public void DeleteDeviceSensor(int deviceId,int deviceSensorId)
        {
            var l = new List<int>()
            {
                deviceSensorId
            };
             _aquariumDao.DeleteDeviceSensors(l);
            return;
        }
    }
}
