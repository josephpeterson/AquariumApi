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
    public partial interface IAquariumService
    {
        #region Device CRUD Operations
        /* Aquarium Device */
        AquariumDevice AddAquariumDevice(AquariumDevice device);
        AquariumDevice GetAquariumDeviceById(int deviceId);
        AquariumDevice GetAquariumDeviceByIpAndKey(string ipAddress, string deviceKey);
        AquariumDevice DeleteAquariumDevice(int deviceId);
        AquariumDevice UpdateAquariumDevice(int userId, AquariumDevice aquariumDevice);
        #endregion

        #region Device Schedules
        List<DeviceSchedule> GetDeviceSchedulesByAccountId(int id);
        void DeleteDeviceSchedule(int deviceId,int scheduleId);
        DeviceSchedule UpdateDeviceSchedule(DeviceSchedule deviceSchedule);
        ScheduledJob PerformScheduleTask(int deviceId, DeviceScheduleTask deviceScheduleTask);
        ScheduleState GetDeviceScheduleStatus(int deviceId);
        ScheduledJob UpsertDeviceScheduledJob(ScheduledJob scheduledJob);
        List<ScheduledJob> GetDeviceScheduledJobs(int deviceId, PaginationSliver pagination);
        #endregion

        #region Device Hardware
        [System.Obsolete]
        AquariumDevice ScanHardware(int deviceId);
        AquariumDevice ApplyAquariumDeviceHardware(int deviceId, AquariumDevice updatedDevice);
        AquariumDevice UpdateDeviceCameraConfiguration(CameraConfiguration config);
        #endregion

        #region Auto Top Off
        #endregion

        #region Device Sensors
        /* Device Sensors */
        DeviceSensor UpdateDeviceSensor(DeviceSensor deviceSensor);
        void DeleteDeviceSensor(int deviceId, int deviceSensorId);
        ICollection<DeviceSensor> GetDeviceSensors(int deviceId);
        DeviceSensor CreateDeviceSensor(int deviceId, DeviceSensor deviceSensor);
        #endregion
        #region Device Tasks
        DeviceScheduleTask CreateDeviceTask(int deviceId, DeviceScheduleTask deviceTask);
        void DeleteDeviceTask(int deviceId, int taskId);
        Task<DeviceSensorTestRequest> TestDeviceSensor(DeviceSensorTestRequest testRequest);
        void AttemptAuthRenewDevice(int deviceId);
        void DeleteWaterATOs(List<int> waterATOIds);
        #endregion
    }
    public partial class AquariumService : IAquariumService
    {
        #region Device CRUD Operations
        public AquariumDevice AddAquariumDevice(AquariumDevice device)
        {
            var newDevice = _aquariumDao.AddAquariumDevice(device);
            //_deviceService.SetAquarium(newDevice.Id, device.AquariumId);
            return newDevice;
        }
        public AquariumDevice GetAquariumDeviceById(int deviceId)
        {
            return _aquariumDao.GetAquariumDeviceById(deviceId);
        }
        public AquariumDevice GetAquariumDeviceByIpAndKey(string ipAddress, string deviceKey)
        {
            return _aquariumDao.GetAquariumDeviceByIpAndKey(ipAddress, deviceKey);
        }
        public AquariumDevice DeleteAquariumDevice(int deviceId)
        {
            return _aquariumDao.DeleteAquariumDevice(deviceId);
        }
        public AquariumDevice UpdateAquariumDevice(int userId, AquariumDevice device)
        {
            var updatedDevice = _aquariumDao.UpdateAquariumDevice(device);
            try
            {
                ApplyUpdatedDevice(updatedDevice.Id);
            }
            catch (Exception ex)
            {
                //Could not  tell devices that we updated
                _logger.LogError("Could not send update to devices.");
                _notificationService.EmitAsync(userId, "Aquarium Device", $"[{device.Name}] Unable to connect to aquarium device. Your device may be offline. " +
                    $"We attempted to contact: " +
                    $"${device.Address}:{device.Port}").Wait();
            }
            return updatedDevice;
        }
        #endregion
        #region Device Common
        public void AttemptAuthRenewDevice(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            _deviceClient.RenewDevice();
        }
        
        #endregion
        #region Device Schedules
        /* Device Schedule */
        public List<DeviceSchedule> GetDeviceSchedulesByAccountId(int id)
        {
            return _aquariumDao.GetDeviceSchedulesByAccount(id);
        }
        public void DeleteDeviceSchedule(int deviceId,int scheduleId)
        {
            //todo verify schedule is on this device
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _aquariumDao.DeleteDeviceSchedule(scheduleId);
            try
            {
                ApplyUpdatedDevice(deviceId);
            }
            catch (Exception e)
            {
                //todo could not update schedule assignment (pi is offline maybe)
            }
        }
        public DeviceSchedule UpdateDeviceSchedule(DeviceSchedule deviceSchedule)
        {
            var updatedSchedule = _aquariumDao.UpdateDeviceSchedule(deviceSchedule);


            var device = _aquariumDao.GetAquariumDeviceById(deviceSchedule.DeviceId);
            try
            {
                ApplyUpdatedDevice(device.Id);
            }
            catch (Exception e)
            {
                //todo could not update schedule assignment (pi is offline maybe)
            }
            return updatedSchedule;
        }
        public ScheduledJob PerformScheduleTask(int deviceId, DeviceScheduleTask deviceScheduleTask)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);

            var scheduledJob = new ScheduledJob()
            {
                DeviceId = deviceId,
                TaskId = deviceScheduleTask.Id.Value,
            };
            scheduledJob = _aquariumDao.UpsertDeviceScheduledJob(scheduledJob);
            var runningJob = _deviceClient.PerformScheduleTask(scheduledJob);
            return runningJob;
        }

        public ScheduleState GetDeviceScheduleStatus(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            return _deviceClient.GetDeviceScheduleStatus();
        }
        
        public List<ScheduledJob> GetDeviceScheduledJobs(int deviceId,PaginationSliver pagination)
        {
            return _aquariumDao.GetDeviceScheduledJobs(deviceId, pagination);
        }
        public ScheduledJob UpsertDeviceScheduledJob(ScheduledJob scheduledJob)
        {
            return _aquariumDao.UpsertDeviceScheduledJob(scheduledJob);
        }

        #endregion
        #region Device Hardware
        /* Device Camera configuration */
        public AquariumDevice ScanHardware(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            throw new NotImplementedException();
            //return _deviceClient.ScanHardware();
        }
        public AquariumDevice UpdateDeviceCameraConfiguration(CameraConfiguration config)
        {
            var deviceToUpdate = _aquariumDao.GetAquariumDeviceById(0);
            deviceToUpdate.CameraConfiguration = config;
            var device = _aquariumDao.UpdateAquariumDevice(deviceToUpdate);

            //tell the pi
            try
            {
                ApplyUpdatedDevice(device.Id);
            }
            catch
            {
                _logger.LogError("Could not send update to devices.");
            }
            return device;
        }
        public AquariumDevice ApplyAquariumDeviceHardware(int deviceId, AquariumDevice updatedDevice)
        {
            return _aquariumDao.ApplyAquariumDeviceHardware(deviceId, updatedDevice);
        }
        #endregion

        #region Device Auto Top Off
        #endregion

        #region Device Sensors
        /* Device Sensors */
        public DeviceSensor CreateDeviceSensor(int deviceId, DeviceSensor deviceSensor)
        {
            deviceSensor.DeviceId = deviceId;
            deviceSensor = _aquariumDao.AddDeviceSensor(deviceSensor);

            //tell the pi
            try
            {
                ApplyUpdatedDevice(deviceId);
            }
            catch
            {
                _logger.LogError("Could not send update to devices.");
            }
            return deviceSensor;
        }
        public ICollection<DeviceSensor> GetDeviceSensors(int deviceId)
        {
            var deviceSensors = _aquariumDao.GetDeviceSensors(deviceId)
                .OrderBy(s => s.Name).ToList();
            return deviceSensors;
        }
        public DeviceSensor UpdateDeviceSensor(DeviceSensor deviceSensor)
        {
            var updated = _aquariumDao.UpdateDeviceSensor(deviceSensor);
            //tell the pi
            try
            {
                ApplyUpdatedDevice(updated.DeviceId);
            }
            catch
            {
                _logger.LogError("Could not send update to devices.");
            }
            return updated;
        }
        public void DeleteDeviceSensor(int deviceId, int deviceSensorId)
        {
            var l = new List<int>()
            {
                deviceSensorId
            };
            _aquariumDao.DeleteDeviceSensors(l);

            //tell the pi
            try
            {
                ApplyUpdatedDevice(deviceId);
            }
            catch
            {
                _logger.LogError("Could not send update to devices.");
            }
            return;
        }
        public Task<DeviceSensorTestRequest> TestDeviceSensor(DeviceSensorTestRequest testRequest)
        {
            var device = _aquariumDao.GetAquariumDeviceById(testRequest.DeviceId);
            _deviceClient.Configure(device);
            return _deviceClient.TestDeviceSensor(testRequest);
        }
        #endregion
        #region Device Tasks
        public DeviceScheduleTask CreateDeviceTask(int deviceId, DeviceScheduleTask deviceTask)
        {
            if(deviceTask.Id.HasValue)
                deviceTask = _aquariumDao.UpdateDeviceTask(deviceTask);
            else
                deviceTask = _aquariumDao.AddDeviceTask(deviceTask);

            //tell the pi
            try
            {
                ApplyUpdatedDevice(deviceId);
            }
            catch
            {
                _logger.LogError("Could not send update to devices.");
            }
            return deviceTask;
        }
        public void DeleteDeviceTask(int deviceId, int taskId)
        {
            var task = _aquariumDao.GetAquariumDeviceById(deviceId).Tasks
                .First(t => t.Id.Value == taskId);
            _aquariumDao.DeleteDeviceTask(task);
            //tell the pi
            try
            {
                ApplyUpdatedDevice(deviceId);
            }
            catch
            {
                _logger.LogError("Could not send update to devices.");
            }
        }
        #endregion
        private void ApplyUpdatedDevice(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            var aq = _aquariumDao.GetAssignedAquariumByDeviceId(device.Id);
            _deviceClient.ApplyAssignedAquarium(aq);
        }
    }
}