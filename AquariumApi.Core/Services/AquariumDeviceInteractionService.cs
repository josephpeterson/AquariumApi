using AquariumApi.DataAccess;
using AquariumApi.DataAccess.Validators;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IAquariumDeviceInteractionService
    {
        //DeviceSensorController
        Task<List<DeviceSensor>> GetDeviceSensorValues(int aquariumId);
        Task<List<DeviceSensor>> UpsertDeviceSensor(int aquariumId, DeviceSensor deviceSensor);
        Task<List<DeviceSensor>> DeleteDeviceSensor(int aquariumId, DeviceSensor deviceSensor);
        Task<DeviceSensorTestRequest> TestDeviceSensor(int aquariumId,DeviceSensorTestRequest testRequest);


        void ClearDeviceLog(int aquariumId);
        Task<DeviceInformation> GetDeviceInformation(int deviceId);
        string GetDeviceLog(int aquariumId);
        List<KeyValuePair<string, string>> GetSelectOptionsBySelectType(int aquariumId, string selectType);
        Task<ScheduleState> StartDeviceSchedule(int aquariumId);
        Task<ScheduleState> StopDeviceSchedule(int aquariumId);
        Task<RunningScheduledJob> PerformDeviceTask(int aquariumId,DeviceScheduleTask deviceScheduleTask);
        Task<ScheduledJob> StopScheduledJob(int aquariumId, ScheduledJob scheduledJob);
        Task<AquariumMixingStationStatus> GetMixingStationStatus(int aquariumId);
    }
    public class AquariumDeviceInteractionService : IAquariumDeviceInteractionService
    {
        private readonly IAquariumService _aquariumService;
        public IDeviceClient _deviceClient { get; }

        public AquariumDeviceInteractionService(IAquariumService aquariumService,IDeviceClient deviceClient)
        {
            _aquariumService = aquariumService;
            _deviceClient = deviceClient;
        }
        private void Configure(int aquariumId)
        {
            var aquarium = _aquariumService.GetAquariumById(aquariumId);
            _deviceClient.Configure(aquarium.Device);
        }

        public async Task<DeviceInformation> GetDeviceInformation(int deviceId)
        {
            Configure(deviceId);
            return await _deviceClient.PingDevice();
        }
        public List<KeyValuePair<string, string>> GetSelectOptionsBySelectType(int aquariumId, string selectType)
        {
            Configure(aquariumId);
            return _deviceClient.GetSelectOptionsBySelectType(selectType);
        }
        public string GetDeviceLog(int aquariumId)
        {
            Configure(aquariumId);
            return _deviceClient.GetDeviceLog();
        }
        public void ClearDeviceLog(int aquariumId)
        {
            Configure(aquariumId);
            _deviceClient.ClearDeviceLog();
        }
        public async Task<List<DeviceSensor>> GetDeviceSensorValues(int aquariumId)
        {
            Configure(aquariumId);
            var res = await _deviceClient.GetDeviceSensorValues();
            return res;
        }
        public async Task<List<DeviceSensor>> UpsertDeviceSensor(int aquariumId, DeviceSensor deviceSensor)
        {
            Configure(aquariumId);
            var res = await _deviceClient.UpsertDeviceSensor(deviceSensor);
            return res;
        }
        public async Task<List<DeviceSensor>> DeleteDeviceSensor(int aquariumId, DeviceSensor deviceSensor)
        {
            Configure(aquariumId);
            var res = await _deviceClient.DeleteDeviceSensor(deviceSensor);
            return res;
        }
        public async Task<DeviceSensorTestRequest> TestDeviceSensor(int aquariumId, DeviceSensorTestRequest testRequest)
        {
            Configure(aquariumId);
            var res = await _deviceClient.TestDeviceSensor(testRequest);
            return res;
        }

        public async Task<ScheduleState> StartDeviceSchedule(int aquariumId)
        {
            Configure(aquariumId);
            var res = await _deviceClient.StartDeviceSchedule();
            return res;
        }

        public async Task<ScheduleState> StopDeviceSchedule(int aquariumId)
        {
            Configure(aquariumId);
            var res = await _deviceClient.StopDeviceSchedule();
            return res;
        }

        public async Task<RunningScheduledJob> PerformDeviceTask(int aquariumId, DeviceScheduleTask deviceScheduleTask)
        {
            Configure(aquariumId);
            var res = await _deviceClient.PerformDeviceTask(deviceScheduleTask);
            return res;
        }

        public async Task<ScheduledJob> StopScheduledJob(int aquariumId, ScheduledJob scheduledJob)
        {
            Configure(aquariumId);
            var res = await _deviceClient.StopScheduledJob(scheduledJob);
            return res;
        }
        public async Task<AquariumMixingStationStatus> GetMixingStationStatus(int aquariumId)
        {
            Configure(aquariumId);
            var res = await _deviceClient.GetMixingStationStatus();
            return res;
        }
    }
}