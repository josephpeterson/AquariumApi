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
using Newtonsoft.Json;

namespace AquariumApi.Controllers
{
    /*
     * These controllers will accept aquariums as a filter.
     * 
     */
    [Authorize]
    public class AquariumsController : Controller
    {
        private readonly IAccountService _accountService;
        public readonly IAquariumService _aquariumService;
        public readonly IDeviceClient _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public AquariumsController(IAccountService accountService,IAquariumService aquariumService, IDeviceClient deviceService,ILogger<DeviceController> logger)
        {
            _accountService = accountService;
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _logger = logger;
        }
        [HttpGet]
        [Route("/v1/Aquariums/All")]
        public IActionResult GetAquariumsIndex()
        {
            try
            {
                var id = _accountService.GetCurrentUserId();


                var aquariums = _aquariumService.GetAquariumsByAccountId(id);
                _logger.LogInformation($"GET /v1/Aquariums called");
                return new OkObjectResult(aquariums);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquariums endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}