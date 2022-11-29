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

namespace AquariumApi.Controllers
{
    public class DeviceSensorController : Controller
    {
        public readonly ILogger<DeviceSensorController> _logger;
        private readonly IGpioService _gpioService;
        public DeviceSensorController(ILogger<DeviceSensorController> logger,
            IGpioService gpioService)
        {
            _logger = logger;
            _gpioService = gpioService;
        }
        [HttpGet(DeviceOutboundEndpoints.SENSOR_RETRIEVE)]
        public IActionResult GetSensors()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SENSOR_RETRIEVE} called");
                var deviceSensors = _gpioService.GetAllSensors();
                return new OkObjectResult(deviceSensors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.SENSOR_RETRIEVE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost(DeviceOutboundEndpoints.SENSOR_UPDATE)]
        public IActionResult ApplyDeviceSensors([FromBody] List<DeviceSensor> sensors)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SENSOR_UPDATE} called");
                var deviceSensors = _gpioService.ApplyDeviceSensors(sensors);
                return new OkObjectResult(deviceSensors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SENSOR_UPDATE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        //Test Device Sensor (this has the correct excception handling mechanism)
        [HttpPost]
        [Route(DeviceOutboundEndpoints.SENSOR_TEST)]
        public async Task<IActionResult> TestDeviceSensor([FromBody] DeviceSensorTestRequest testRequest)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SENSOR_TEST} called");
                var finishedRequest = _gpioService.TestDeviceSensor(testRequest);
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
                _logger.LogError($"POST {DeviceOutboundEndpoints.SENSOR_TEST}: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new AquariumServiceException("Unknown error occured"));
            }
        }

    }
}