using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [ApiController]
    [DisableCors]
    public class ClientAppController : Controller
    {
        private IDeviceService _deviceService;
        private ScheduleService _scheduleService;
        private IHardwareService _hardwareService;
        private IAquariumClient _aquariumClient;
        private ILogger<ClientAppController> _logger;
        private IConfiguration _config;

        public ClientAppController(IDeviceService deviceService,
            ScheduleService scheduleService,
            IHardwareService hardwareService,
            IAquariumClient aquariumClient,
            ILogger<ClientAppController> logger, IConfiguration config)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _hardwareService = hardwareService;
            _aquariumClient = aquariumClient;
            _logger = logger;
            _config = config;
        }
        [HttpPost]
        [Route("/ClientApp/Login")]
        public IActionResult AttemptLogin([FromBody] DeviceLoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation("POST /ClientApp/Login called");
                var data = _aquariumClient.RetrieveLoginToken(loginRequest).Result;
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/ClientApp/Login caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet]
        [Route("ClientApp")]
        public IActionResult GetDetailedInformation()
        {
            try
            {
                _logger.LogInformation("GET /ClientApp called");
                var data = _aquariumClient.ValidateAuthenticationToken().Result;
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/ClientApp caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return Unauthorized();
            }
        }
        [HttpDelete]
        [Route("ClientApp/Logout")]
        public IActionResult Logout()
        {
            try
            {
                _logger.LogInformation("DELETE /ClientApp/Logout called");
                _aquariumClient.ClearLoginToken();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE /v1/ClientApp/Logout caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
    }
}
