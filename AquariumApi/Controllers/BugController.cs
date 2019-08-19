using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AquariumApi.Controllers
{
    public class BugController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly IDeviceService _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public BugController(IAquariumService aquariumService, IDeviceService deviceService, ILogger<DeviceController> logger)
        {
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _logger = logger;
        }
        [HttpPost,Route("/v1/Bug/Submit")]
        public IActionResult SubmitBugReport(BugReport report)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Bug/Submit called");
                var r = _aquariumService.SubmitBugReport(report);
                return new OkObjectResult(r);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Bug/Submit endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet, Route("/v1/Bugs"),Authorize(Roles = "Administrator,Developer")]
        public IActionResult GetAllBugs()
        {
            try
            {
                _logger.LogInformation($"POST /v1/Bugs called");
                List<BugReport> bugs = _aquariumService.GetAllBugs();
                return new OkObjectResult(bugs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Bugs endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}