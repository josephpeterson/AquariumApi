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
        Task<List<DeviceSensor>> ApplyDeviceSensors(int aquariumId, List<DeviceSensor> deviceSensors);
        void ClearDeviceLog(int aquariumId);
        DeviceInformation GetDeviceInformation(int deviceId);
        string GetDeviceLog(int aquariumId);
        List<KeyValuePair<string, string>> GetSelectOptionsBySelectType(int aquariumId, string selectType);
        Task<DeviceSensorTestRequest> TestDeviceSensor(int aquariumId,DeviceSensorTestRequest testRequest);
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

        public DeviceInformation GetDeviceInformation(int deviceId)
        {
            Configure(deviceId);
            return _deviceClient.PingDevice();
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
        public async Task<DeviceSensorTestRequest> TestDeviceSensor(int aquariumId,DeviceSensorTestRequest testRequest)
        {
            Configure(aquariumId);
            var res = await _deviceClient.TestDeviceSensor(testRequest);
            return res;
        }
        public async Task<List<DeviceSensor>> ApplyDeviceSensors(int aquariumId, List<DeviceSensor> deviceSensors)
        {
            Configure(aquariumId);
            var res = await _deviceClient.ApplyDeviceSensors(deviceSensors);
            return res;
        }
    }
}