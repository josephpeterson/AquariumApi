﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
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
        public readonly IDeviceClient _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public BugController(IAquariumService aquariumService, IAccountService accountService,IDeviceClient deviceService, ILogger<DeviceController> logger,IEmailerService emailerService)
        {
            _accountService = accountService;
            _emailerService = emailerService;
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _logger = logger;
        }
        [HttpPost,Route(AquariumApiEndpoints.BUG_SUBMIT)]
        public IActionResult SubmitBugReport([FromBody] BugReport report)
        {
            //Static information
            report.ImpactedUserId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.BUG_SUBMIT} called");
                var r = _aquariumService.SubmitBugReport(report);
                return new OkObjectResult(r);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.BUG_SUBMIT} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
    }
}