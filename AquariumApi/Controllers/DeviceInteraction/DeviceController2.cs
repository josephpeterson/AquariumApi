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
    [Route("DeviceInteraction")]
    public class DeviceController2 : Controller
    {
        private readonly IAccountService _accountService;
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<DeviceController> _logger;
        public readonly IAquariumDeviceInteractionService _deviceService;
        public DeviceController2(IAccountService accountService
            ,IAquariumService aquariumService
            ,IAquariumDeviceInteractionService deviceService
            ,ILogger<DeviceController> logger)
        {
            _accountService = accountService;
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _logger = logger;
        }
        [HttpGet]
        [Route("{aquariumId}/" + DeviceOutboundEndpoints.PING)]
        public async Task<IActionResult> GetDeviceById(int aquariumId)
        {
            if (!ValidateRequest(aquariumId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST DeviceInteraction/{aquariumId}/{DeviceOutboundEndpoints.PING} called");
                var deviceInformation = await _deviceService.GetDeviceInformation(aquariumId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST DeviceInteraction/{aquariumId}/{DeviceOutboundEndpoints.PING} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpGet]
        [Route("{aquariumId}/" + DeviceOutboundEndpoints.SELECT_FORM_TYPES)]
        public IActionResult GetSelectOptionsByType(int aquariumId,string selectType)
        {
            if (!ValidateRequest(aquariumId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST DeviceInteraction/{aquariumId}/{DeviceOutboundEndpoints.SELECT_FORM_TYPES}/{selectType} called");
                var formSelectOptions = _deviceService.GetSelectOptionsBySelectType(aquariumId,selectType);
                return new OkObjectResult(formSelectOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST DeviceInteraction/{aquariumId}/{DeviceOutboundEndpoints.SELECT_FORM_TYPES}/{selectType} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        private bool ValidateRequest(int aquariumId)
        {
            var id = _accountService.GetCurrentUserId();
            var aquariums = _aquariumService.GetAquariumsByAccountId(id);
            return aquariums.Where(a => a.Id == aquariumId && a.Device != null).Any();
        }
    }
}