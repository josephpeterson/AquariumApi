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

    public class DeviceATOController : Controller
    {
        public readonly IAquariumService _aquariumService;
        private readonly IAccountService _accountService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceATOController(IAquariumService aquariumService, IAccountService accountService, ILogger<DeviceController> logger)
        {
            _aquariumService = aquariumService;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_ATO_STATUS)]
        public IActionResult GetDeviceATOStatus(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"GET {AquariumApiEndpoints.DEVICE_ATO_STATUS.AggregateParams($"{deviceId}")} called");
                var atoStatus = _aquariumService.GetDeviceATOStatus(deviceId);
                return new OkObjectResult(atoStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {AquariumApiEndpoints.DEVICE_ATO_STATUS.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_ATO_HISTORY)]
        public IActionResult GetDeviceATOHistory(int deviceId,[FromBody] PaginationSliver pagination = null)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            if (pagination == null)
                pagination = new PaginationSliver();
            try
            {
                _logger.LogInformation($"GET {AquariumApiEndpoints.DEVICE_ATO_HISTORY.AggregateParams($"{deviceId}")} called");
                List<ATOStatus> atoHistory = _aquariumService.GetDeviceATOHistory(deviceId, pagination);
                return new OkObjectResult(atoHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {AquariumApiEndpoints.DEVICE_ATO_HISTORY.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_ATO_RUN)]
        public IActionResult RunDeviceATO(int deviceId,[FromBody] int maxRuntime)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_ATO_RUN.AggregateParams($"{deviceId}")} called");
                var atoStatus = _aquariumService.PerformDeviceATO(deviceId, maxRuntime);
                return new OkObjectResult(atoStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_ATO_RUN.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_ATO_STOP)]
        public IActionResult StopDeviceATO(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_ATO_STOP.AggregateParams($"{deviceId}")} called");
                var atoStatus = _aquariumService.StopDeviceATO(deviceId);
                return new OkObjectResult(atoStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_ATO_STOP.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        private bool ValidateRequest(int deviceId)
        {
            var id = _accountService.GetCurrentUserId();
            var aquariums = _aquariumService.GetAquariumsByAccountId(id);
            return aquariums.Where(a => a.Device.Id == deviceId).Any();
        }
    }
}