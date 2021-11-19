using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AquariumApi.Controllers
{
    [Authorize]
    public class DeviceController : Controller
    {
        private readonly IAccountService _accountService;
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceController(IAccountService accountService,IAquariumService aquariumService,ILogger<DeviceController> logger)
        {
            _accountService = accountService;
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_RETRIEVE)]
        public IActionResult GetDeviceById(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_RETRIEVE.AggregateParams($"{deviceId}")} called");
                var device = _aquariumService.GetAquariumDeviceById(deviceId);
                return new OkObjectResult(device);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_RETRIEVE.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_DELETE)]
        public IActionResult DeleteAquariumDevice(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST  {AquariumApiEndpoints.DEVICE_DELETE.AggregateParams($"{deviceId}")} called");
                _aquariumService.DeleteAquariumDevice(deviceId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST  {AquariumApiEndpoints.DEVICE_DELETE.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_UPDATE)]
        public IActionResult UpdateAquariumDevice([FromBody] AquariumDevice device)
        {
            if (!ValidateRequest(device.Id))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_UPDATE} called");
                var id = _accountService.GetCurrentUserId();
                var aq = _aquariumService.GetAquariumById(device.AquariumId);
                if(!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var updatedDevice = _aquariumService.UpdateAquariumDevice(id,device);
                return new OkObjectResult(updatedDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_UPDATE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_CREATE)]
        public IActionResult AddAquariumDevice([FromBody] AquariumDevice device)
        {
            //todo validate
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_CREATE} called");
                var newDevice = _aquariumService.AddAquariumDevice(device);
                return CreatedAtAction(nameof(UpdateAquariumDevice), new { id = newDevice.Id }, newDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_CREATE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_DISPATCH_SCAN)]
        public IActionResult ScanAquariumDeviceHardware(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_SCAN.AggregateParams($"{deviceId}")} called");
                var updatedDevice = _aquariumService.ScanHardware(deviceId);
                var deviceToUpdate = _aquariumService.ApplyAquariumDeviceHardware(deviceId,updatedDevice);
                return new OkObjectResult(deviceToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_SCAN.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_DISPATCH_PING)]
        public IActionResult PingAquariumDevice(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {

                _logger.LogInformation($"GET {AquariumApiEndpoints.DEVICE_DISPATCH_PING.AggregateParams($"{deviceId}")} called");
                var deviceInformation = _aquariumService.PingDevice(deviceId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {AquariumApiEndpoints.DEVICE_DISPATCH_PING.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_DISPATCH_SNAPSHOT_CONFIGURATION)]
        public IActionResult UpdateCameraConfiguration(int deviceId,[FromBody] CameraConfiguration config)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_SNAPSHOT_CONFIGURATION.AggregateParams($"{deviceId}")} called");
                var aquariumDevice = _aquariumService.UpdateDeviceCameraConfiguration(config);
                return new OkObjectResult(aquariumDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_SNAPSHOT_CONFIGURATION.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

       //Retrieve AquariumDeviceApi.log
        [HttpPost, DisableRequestSizeLimit]
        [Route(AquariumApiEndpoints.DEVICE_LOG)]
        public IActionResult ClearDeviceLog(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_LOG.AggregateParams($"{deviceId}")} called");
                var deviceLog = _aquariumService.GetDeviceLog(deviceId);
                return new OkObjectResult(deviceLog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_LOG.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        
        [HttpPost, DisableRequestSizeLimit]
        [Route(AquariumApiEndpoints.DEVICE_LOG_CLEAR)]
        public IActionResult GetDeviceLog(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_LOG_CLEAR.AggregateParams($"{deviceId}")} called");
                _aquariumService.ClearDeviceLog(deviceId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_LOG_CLEAR.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        
        //Retrieve Assigned aquarium/schedule information
        [HttpPost, DisableRequestSizeLimit]
        [Route(AquariumApiEndpoints.DEVICE_RETRIEVE_DETAILED)]
        public IActionResult GetDeviceInformation(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_RETRIEVE_DETAILED.AggregateParams($"{deviceId}")} called");
                var deviceInformation = _aquariumService.GetDeviceInformation(deviceId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_RETRIEVE_DETAILED.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        //Retrieve Assigned aquarium/schedule information
        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_UPDATE_CONFIGURATION)]
        public IActionResult UpdateConfigurationFile(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_UPDATE_CONFIGURATION.AggregateParams($"{deviceId}")} called");
                var deviceInformation = _aquariumService.GetDeviceInformation(deviceId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_UPDATE_CONFIGURATION.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        #region Device Sensors
        //Create/remove device sensors
        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_SENSORS)]
        public IActionResult GetDeviceSensors(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_SENSORS.AggregateParams($"{deviceId}")} called");
                var deviceSensors = _aquariumService.GetDeviceSensors(deviceId);
                return new OkObjectResult(deviceSensors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_SENSORS.AggregateParams($"{deviceId}")}s: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_SENSOR_CREATE)]
        public IActionResult CreateDeviceSensor(int deviceId, [FromBody] DeviceSensor sensor)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_SENSOR_CREATE.AggregateParams($"{deviceId}")} called");
                var deviceSensor = _aquariumService.CreateDeviceSensor(deviceId,sensor);
                return new OkObjectResult(deviceSensor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_SENSOR_CREATE.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_SENSOR_DELETE)]
        public IActionResult RemoveDeviceSensor(int deviceId,[FromBody] DeviceSensor sensor)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_SENSOR_DELETE.AggregateParams($"{deviceId}")} called");
                _aquariumService.DeleteDeviceSensor(deviceId,sensor.Id);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_SENSOR_DELETE.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPut]
        [Route(AquariumApiEndpoints.DEVICE_SENSOR_UPDATE)]
        public IActionResult UpdateDeviceSensor(int deviceId, [FromBody] DeviceSensor sensor)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_SENSOR_UPDATE.AggregateParams($"{deviceId}")} called");
                sensor = _aquariumService.UpdateDeviceSensor(sensor);
                return new OkObjectResult(sensor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_SENSOR_UPDATE.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        //Test Device Sensor (this has the correct excception handling mechanism)
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_SENSOR_TEST)]
        public async Task<IActionResult> TestDeviceSensor([FromBody] DeviceSensorTestRequest testRequest)
        {
            if (!ValidateRequest(testRequest.DeviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_SENSOR_TEST} called");
                var finishedRequest = await _aquariumService.TestDeviceSensor(testRequest);
                return new OkObjectResult(finishedRequest);
            }
            //Friendly exceptions
            catch(BaseException ex)
            {
                _logger.LogInformation($"Handled Exception: {ex.Message} {ex.Source}");
                return BadRequest(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_SENSOR_TEST}: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new AquariumServiceException("Unknown error occured"));
            }
        }

        #endregion



        #region Device Schedules
        //Deploy/Remove Device Schedules
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_SCHEDULE_DEPLOY)]
        public IActionResult DeploySchedule(int deviceId,int scheduleId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_SCHEDULE_DEPLOY.AggregateParams($"{deviceId}", $"{scheduleId}")} called");
                var scheduleAssignment = _aquariumService.DeployDeviceSchedule(deviceId, scheduleId);
                return new OkObjectResult(scheduleAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_SCHEDULE_DEPLOY.AggregateParams($"{deviceId}", $"{scheduleId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_SCHEDULE_REMOVE)]
        public IActionResult RemoveSchedule(int deviceId, int scheduleId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_SCHEDULE_REMOVE.AggregateParams($"{deviceId}", $"{scheduleId}")} called");
                var scheduleAssignment = _aquariumService.RemoveDeviceSchedule(deviceId, scheduleId);
                return new OkObjectResult(scheduleAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_SCHEDULE_REMOVE.AggregateParams($"{deviceId}", $"{scheduleId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_SCHEDULE_STATUS)]
        public IActionResult GetScheduleStatus(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_SCHEDULE_STATUS.AggregateParams($"{deviceId}")} called");
                ScheduleState scheduleState = _aquariumService.GetDeviceScheduleStatus(deviceId);
                return new OkObjectResult(scheduleState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_SCHEDULE_STATUS.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_DISPATCH_TASK)]
        public IActionResult PerformScheduleTask(int deviceId,[FromBody] DeviceScheduleTask deviceScheduleTask)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_TASK.AggregateParams($"{deviceId}")} called");
                _aquariumService.PerformScheduleTask(deviceId,deviceScheduleTask);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_TASK.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(ex.Message);
            }
        }

        #endregion


        private bool ValidateRequest(int deviceId)
        {
            var id = _accountService.GetCurrentUserId();
            var aquariums = _aquariumService.GetAquariumsByAccountId(id);
            return aquariums.Where(a => a.Device.Id == deviceId).Any();
        }
    }
}