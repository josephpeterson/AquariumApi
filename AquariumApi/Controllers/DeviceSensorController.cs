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

namespace AquariumApi.Controllers
{
    [Authorize]

    public class DeviceSensorController : Controller
    {
        public readonly IAquariumService _aquariumService;
        private readonly IAccountService _accountService;
        public readonly IDeviceClient _deviceService;
        public readonly ILogger<DeviceSensorController> _logger;
        public DeviceSensorController(IAquariumService aquariumService, IAccountService accountService, IDeviceClient deviceService, ILogger<DeviceSensorController> logger)
        {
            _aquariumService = aquariumService;
            _accountService = accountService;
            _deviceService = deviceService;
            _logger = logger;
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
                var deviceSensor = _aquariumService.CreateDeviceSensor(deviceId, sensor);
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
        public IActionResult RemoveDeviceSensor(int deviceId, [FromBody] DeviceSensor sensor)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_SENSOR_DELETE.AggregateParams($"{deviceId}")} called");
                _aquariumService.DeleteDeviceSensor(deviceId, sensor.Id);
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
            catch (BaseException ex)
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
        private bool ValidateRequest(int deviceId)
        {
            var id = _accountService.GetCurrentUserId();
            var aquariums = _aquariumService.GetAquariumsByAccountId(id);
            return aquariums.Where(a => a.Device.Id == deviceId).Any();
        }

    }
}