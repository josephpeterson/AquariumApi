using System;
using System.Collections.Generic;
using System.IO;
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
                var data = _aquariumClient.PingAquariumService().Result;
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/ClientApp caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return Unauthorized();
            }
        }
        [HttpGet]
        [Route("ClientApp/Schedule")]
        public IActionResult GetScheduleInformation()
        {
            try
            {
                _logger.LogInformation($"GET /v1/ClientApp/Schedule called");
                var scheduleStatus = _scheduleService.GetStatus();
                return new OkObjectResult(scheduleStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/ClientApp/Schedule caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return Unauthorized();
            }
        }
        [HttpGet]
        [Route("ClientApp/Log")]
        public IActionResult GetDeviceLog()
        {
            try
            {
                var text = System.IO.File.ReadAllText("AquariumDeviceApi.log");
                return new OkObjectResult(text);
            }
            catch (FileNotFoundException)
            {
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
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
                _scheduleService.StopAsync(_scheduleService.token).Wait();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE /v1/ClientApp/Logout caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpPost("ClientApp/PerformTask")]
        public IActionResult PerformTask([FromBody] DeviceScheduleTask deviceScheduleTask)
        {
            try
            {
                _scheduleService.PerformTask(deviceScheduleTask);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/ClientApp/PerformTask endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet]
        [Route("ClientApp/Auth/Renew")]
        public IActionResult RenewAuthenticationToken()
        {
            try
            {
                _logger.LogInformation("GET /ClientApp/Auth/Renew called");
                var res = _deviceService.RenewAuthenticationToken().Result;
                if (res)
                    return Ok();
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/ClientApp/Auth/Renew caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("ClientApp/Hardware/Scan")]
        public IActionResult ApplyDeviceHardware()
        {
            try
            {
                var r = Request;
                _logger.LogInformation("GET /ClientApp/Hardware/Scan called");
                var aquarium = _deviceService.ApplyDeviceHardware().Result;
                if (aquarium != null)
                    return new OkObjectResult(aquarium);
                else
                    return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/ClientApp/Hardware/Scan caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest();
            }
        }

    }
}
