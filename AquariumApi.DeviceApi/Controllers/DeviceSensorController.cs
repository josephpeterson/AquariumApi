using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.DeviceApi;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    public class DeviceSensorController : Controller
    {
        public readonly ILogger<DeviceSensorController> _logger;
        private readonly ScheduleService _scheduleService;
        public DeviceSensorController(ILogger<DeviceSensorController> logger,
            ScheduleService scheduleService)
        {
            _logger = logger;
            _scheduleService = scheduleService;
        }
        /// <summary>
        /// Retrieve real time sensors with values
        /// </summary>
        /// <returns></returns>
        [HttpPost(DeviceOutboundEndpoints.SENSOR_RETRIEVE)]
        public IActionResult GetDeviceSensorValues()
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SENSOR_RETRIEVE} called");
                var deviceSensors = _scheduleService.GetDeviceSensorValues();
                return new OkObjectResult(deviceSensors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SENSOR_RETRIEVE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        /// <summary>
        /// Handle update/creation of device sensors
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        [HttpPut(DeviceOutboundEndpoints.SENSOR_UPDATE)]
        public IActionResult UpsertDeviceSensor([FromBody] DeviceSensor sensor)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SENSOR_UPDATE} called");
                var deviceSensors = _scheduleService.UpsertDeviceSensor(sensor);
                return new OkObjectResult(deviceSensors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SENSOR_UPDATE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        /// <summary>
        /// Handle removal of device sensors
        /// </summary>
        /// <param name="sensor"></param>
        /// <returns></returns>
        [HttpPost(DeviceOutboundEndpoints.SENSOR_DELETE)]
        public IActionResult DeleteDeviceSensor([FromBody] DeviceSensor sensor)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SENSOR_DELETE} called");
                var deviceSensors = _scheduleService.DeleteDeviceSensor(sensor);
                return new OkObjectResult(deviceSensors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SENSOR_DELETE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        /// <summary>
        /// Handle test device sensors
        /// </summary>
        /// <param name="testRequest"></param>
        /// <returns></returns>
        [HttpPost(DeviceOutboundEndpoints.SENSOR_TEST)]
        public IActionResult TestDeviceSensor([FromBody] DeviceSensorTestRequest testRequest)
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SENSOR_TEST} called");
                var finishedRequest = _scheduleService.TestDeviceSensor(testRequest);
                return new OkObjectResult(finishedRequest);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SENSOR_TEST} endpoint caught exception: {ex.Message}");
                var e = new DeviceException(ex.Message);
                return BadRequest(e);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.SENSOR_TEST} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }

    }
}