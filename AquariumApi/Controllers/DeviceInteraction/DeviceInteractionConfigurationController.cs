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
    public class DeviceInteractionConfigurationController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAquariumService _aquariumService;
        private readonly IAquariumDeviceInteractionService _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceInteractionConfigurationController(IAccountService accountService
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
        [HttpGet("{aquariumId} " + DeviceOutboundEndpoints.SYSTEM_LOG_RETRIEVE), DisableRequestSizeLimit]
        public IActionResult GetDeviceLog(int aquariumId)
        {
            if (!ValidateRequest(aquariumId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {aquariumId + DeviceOutboundEndpoints.SYSTEM_LOG_RETRIEVE} called");
                var deviceLog = _deviceService.GetDeviceLog(aquariumId);
                return new OkObjectResult(deviceLog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {aquariumId + DeviceOutboundEndpoints.SYSTEM_LOG_RETRIEVE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        [HttpPost("{aquariumId} " + DeviceOutboundEndpoints.SYSTEM_LOG_CLEAR), DisableRequestSizeLimit]
        public IActionResult ClearDeviceLog(int aquariumId)
        {
            if (!ValidateRequest(aquariumId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {aquariumId + DeviceOutboundEndpoints.SYSTEM_LOG_RETRIEVE} called");
                var deviceLog = _deviceService.GetDeviceLog(aquariumId);
                return new OkObjectResult(deviceLog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {aquariumId + DeviceOutboundEndpoints.SYSTEM_LOG_RETRIEVE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
    }
}