using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.Models;
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
        public readonly IDeviceService _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceATOController(IAquariumService aquariumService, IAccountService accountService, IDeviceService deviceService, ILogger<DeviceController> logger)
        {
            _aquariumService = aquariumService;
            _accountService = accountService;
            _deviceService = deviceService;
            _logger = logger;
        }

        [HttpGet]
        [Route("/v1/Device/{deviceId}/ATO")]
        public IActionResult GetDeviceATOStatus(int deviceId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Device/{deviceId}/ATO/Status called");
                var atoStatus = _deviceService.GetDeviceATOStatus(deviceId);
                return new OkObjectResult(atoStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Device/{deviceId}/ATO/Status endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}