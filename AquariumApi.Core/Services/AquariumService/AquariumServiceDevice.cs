using AquariumApi.DataAccess;
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

        #region Device Common
        DeviceInformation PingDevice(int deviceId);
        string GetDeviceLog(int deviceId);
        void ClearDeviceLog(int deviceId);
        DeviceInformation GetDeviceInformation(int deviceId);
        #endregion

        #region Device Schedules
        List<DeviceSchedule> GetDeviceSchedulesByAccountId(int id);
        void DeleteDeviceSchedule(int scheduleId);
        DeviceSchedule AddDeviceSchedule(DeviceSchedule deviceSchedule);
        List<DeviceScheduleAssignment> DeployDeviceSchedule(int deviceId, int scheduleId);
        List<DeviceScheduleAssignment> RemoveDeviceSchedule(int deviceId, int scheduleId);
        DeviceSchedule UpdateDeviceSchedule(DeviceSchedule deviceSchedule);
        void PerformScheduleTask(int deviceId,DeviceScheduleTask deviceScheduleTask);
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
        /* ATO */
        ATOStatus GetDeviceATOStatus(int deviceId);
        ATOStatus UpdateDeviceATOStatus(ATOStatus atoStatus);
        List<ATOStatus> GetDeviceATOHistory(int deviceId, PaginationSliver paginationSliver);
        ATOStatus PerformDeviceATO(int deviceId,int maxRuntime);
        ATOStatus StopDeviceATO(int deviceId);
        #endregion

        #region Device Sensors
        /* Device Sensors */
        DeviceSensor UpdateDeviceSensor(DeviceSensor deviceSensor);
        void DeleteDeviceSensor(int deviceId, int deviceSensorId);
        ICollection<DeviceSensor> GetDeviceSensors(int deviceId);
        DeviceSensor CreateDeviceSensor(int deviceId, DeviceSensor deviceSensor);
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
                _deviceClient.Configure(updatedDevice);
                _deviceClient.ApplyUpdatedDevice(updatedDevice);
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
        public DeviceInformation PingDevice(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            return _deviceClient.PingDevice();
        }
        public string GetDeviceLog(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            return _deviceClient.GetDeviceLog();
        }
        public void ClearDeviceLog(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            _deviceClient.ClearDeviceLog();
        }
        public DeviceInformation GetDeviceInformation(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            return _deviceClient.PingDevice();
        }
        #endregion
        #region Device Schedules
        /* Device Schedule */
        public List<DeviceSchedule> GetDeviceSchedulesByAccountId(int id)
        {
            return _aquariumDao.GetDeviceSchedulesByAccount(id);
        }
        public void DeleteDeviceSchedule(int scheduleId)
        {
            var affectedDevices = _aquariumDao.GetDevicesInUseBySchedule(scheduleId);
            _aquariumDao.DeleteDeviceSchedule(scheduleId);
            affectedDevices.ForEach(device =>
            {
                try
                {
                    var newDevice = _aquariumDao.GetAquariumDeviceById(device.Id);
                    _deviceClient.Configure(newDevice);
                    _deviceClient.ApplyUpdatedDevice(newDevice);
                    //_deviceClient.ApplyScheduleAssignment(device.Id, _aquariumDao.GetAssignedDeviceSchedules(device.Id).Select(sa => sa.Schedule).ToList());
                }
                catch (Exception e)
                {
                    //todo could not update schedule assignment (pi is offline maybe)
                }
            });

        }
        public DeviceSchedule AddDeviceSchedule(DeviceSchedule deviceSchedule)
        {
            return _aquariumDao.AddDeviceSchedule(deviceSchedule);
        }
        public List<DeviceScheduleAssignment> DeployDeviceSchedule(int deviceId, int scheduleId)
        {
            _aquariumDao.AssignDeviceSchedule(scheduleId, deviceId);
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            try
            {
                _deviceClient.Configure(device);
                _deviceClient.ApplyUpdatedDevice(device);
            }
            catch (Exception e)
            {
                _logger.LogError("Could not apply schedule assignment");
            }
            return device.ScheduleAssignments.ToList();

        }
        public List<DeviceScheduleAssignment> RemoveDeviceSchedule(int deviceId, int scheduleId)
        {
            _aquariumDao.UnassignDeviceSchedule(scheduleId, deviceId);
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            try
            {
                _deviceClient.Configure(device);
                _deviceClient.ApplyUpdatedDevice(device);
            }
            catch (Exception e)
            {
                _logger.LogError("Could not apply schedule assignment");
            }
            return device.ScheduleAssignments.ToList();
        }
        public DeviceSchedule UpdateDeviceSchedule(DeviceSchedule deviceSchedule)
        {
            var updatedSchedule = _aquariumDao.UpdateDeviceSchedule(deviceSchedule);


            var affectedDevices = _aquariumDao.GetDevicesInUseBySchedule(updatedSchedule.Id);
            affectedDevices.ForEach(device =>
            {
                try
                {
                    var d = _aquariumDao.GetAquariumDeviceById(device.Id);
                    _deviceClient.Configure(d);
                    _deviceClient.ApplyUpdatedDevice(d);
                }
                catch (Exception e)
                {
                    //todo could not update schedule assignment (pi is offline maybe)
                }
            });
            return updatedSchedule;
        }
        public void PerformScheduleTask(int deviceId, DeviceScheduleTask deviceScheduleTask)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            _deviceClient.PerformScheduleTask(deviceScheduleTask);
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
        /* ATO */
        public ATOStatus GetDeviceATOStatus(int deviceId)
        {
            try
            {
                //attempt to get status from pi
                var device = _aquariumDao.GetAquariumDeviceById(deviceId);
                _deviceClient.Configure(device);
                var state = _deviceClient.GetDeviceATOStatus();

                //insert into db
                if (state.Id.HasValue)
                    state = _aquariumDao.UpdateATOStatus(state);
                return state;
            }
            catch (Exception e)
            {
                _logger.LogInformation("Could not retrieve ATO status from device. Loading from cache...");
                //load from cache
                return _aquariumDao.GetATOHistory(deviceId, new PaginationSliver
                {
                    Count = 1,
                    Descending = true
                }).FirstOrDefault();
            }
        }
        public ATOStatus UpdateDeviceATOStatus(ATOStatus atoStatus)
        {
            //Get last ATO
            var uncompletedATOs = _aquariumDao.GetATOHistory(atoStatus.DeviceId, new PaginationSliver
            {
                Descending = true,
                Count = 10 //doesn't matter, just not all of them
            }).Where(a => !a.Completed).OrderBy(a => a.UpdatedAt);
            uncompletedATOs.ToList().ForEach(ato =>
            {
                if (ato.Id == atoStatus.Id)
                    return;
                ato.UpdatedAt = DateTime.Now.ToUniversalTime();
                ato.EndReason = "Error"; //todo
                ato.Completed = true;
                _aquariumDao.UpdateATOStatus(ato);
            });
            if (!atoStatus.PumpRunning && atoStatus.Id == null)
                return atoStatus;
            return _aquariumDao.UpdateATOStatus(atoStatus);
        }
        public List<ATOStatus> GetDeviceATOHistory(int deviceId, PaginationSliver paginationSliver)
        {
            return _aquariumDao.GetATOHistory(deviceId, paginationSliver);
        }
        public ATOStatus PerformDeviceATO(int deviceId, int maxRuntime)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            return _deviceClient.PerformDeviceATO(maxRuntime);
        }

        public ATOStatus StopDeviceATO(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            return _deviceClient.StopDeviceATO();
        }
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
        private void ApplyUpdatedDevice(int deviceId)
        {
            var device = _aquariumDao.GetAquariumDeviceById(deviceId);
            _deviceClient.Configure(device);
            _deviceClient.ApplyUpdatedDevice(device);
        }
    }
}