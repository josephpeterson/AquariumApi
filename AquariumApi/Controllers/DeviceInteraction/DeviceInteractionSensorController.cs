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

namespace AquariumApi.Controllers.DeviceInteraction
{
    [Authorize]
    [Route("DeviceInteraction")]
    public class DeviceInteractionSensorController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAquariumService _aquariumService;
        private readonly IAquariumDeviceInteractionService _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceInteractionSensorController(IAccountService accountService
            ,IAquariumService aquariumService
            ,IAquariumDeviceInteractionService deviceService
            ,ILogger<DeviceController> logger)
        {
            _accountService = accountService;
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _logger = logger;
        }
        private bool ValidateRequest(int aquariumId)
        {
            var id = _accountService.GetCurrentUserId();
            var aquariums = _aquariumService.GetAquariumsByAccountId(id);
            return aquariums.Where(a => a.Id == aquariumId && a.Device != null).Any();
        }
        [HttpPost("{aquariumId}" + DeviceOutboundEndpoints.SENSOR_TEST)]
        public async Task<IActionResult> TestDeviceSensor(int aquariumId, [FromBody] DeviceSensorTestRequest testRequest)
        {
            if (!ValidateRequest(aquariumId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST DeviceInteraction/{aquariumId + DeviceOutboundEndpoints.SENSOR_TEST} called");
                var finishedRequest = await _deviceService.TestDeviceSensor(aquariumId,testRequest);
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
                _logger.LogError($"POST DeviceInteraction/{aquariumId + DeviceOutboundEndpoints.SENSOR_TEST}: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new AquariumServiceException("Unknown error occured"));
            }
        }
        [HttpPut("{aquariumId}" + DeviceOutboundEndpoints.SENSOR_UPDATE)]
        public async Task<IActionResult> ApplyDeviceSensors(int aquariumId, [FromBody] List<DeviceSensor> sensors)
        {
            if (!ValidateRequest(aquariumId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST DeviceInteraction/{DeviceOutboundEndpoints.SENSOR_UPDATE} called");
                var deviceSensors = await _deviceService.ApplyDeviceSensors(aquariumId,sensors);
                return new OkObjectResult(deviceSensors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SENSOR_UPDATE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
    }
}