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
    [Authorize]
    public class BugController : Controller
    {
        private IAccountService _accountService;
        private readonly IEmailerService _emailerService;
        public readonly IAquariumService _aquariumService;
        public readonly IDeviceService _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public BugController(IAquariumService aquariumService, IAccountService accountService,IDeviceService deviceService, ILogger<DeviceController> logger,IEmailerService emailerService)
        {
            _accountService = accountService;
            _emailerService = emailerService;
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _logger = logger;
        }
        [HttpPost,Route("/v1/Bug/Submit")]
        public IActionResult SubmitBugReport([FromBody] BugReport report)
        {
            //Static information
            report.ImpactedUserId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier).Value);

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
        [HttpGet, Route("/v1/EmailTest")]
        public IActionResult SendEmail()
        {
             
            
            try
            {
                _logger.LogInformation($"POST /v1/EmailTest called");
                _accountService.SendResetPasswordEmail();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Bug/Submit endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}