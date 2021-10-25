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
        private IAquariumAuthService _aquariumAuthService;
        private ILogger<ClientAppController> _logger;
        private IConfiguration _config;

        public ClientAppController(IDeviceService deviceService,
            ScheduleService scheduleService,
            IAquariumAuthService aquariumAuthService,
            ILogger<ClientAppController> logger, IConfiguration config)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _aquariumAuthService = aquariumAuthService;
            _logger = logger;
            _config = config;
        }
        [HttpGet]
        [Route("ClientApp")]
        public IActionResult GetDetailedInformation()
        {
            try
            {
                _logger.LogInformation("GET /ClientApp called");
                var account = _aquariumAuthService.GetAccount();
                var aquarium = _aquariumAuthService.GetAquarium();

                if (account is null)
                    return Unauthorized();
                return new OkObjectResult(new
                {
                    Account = account,
                    Aquarium = aquarium
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/ClientApp caught exception: { ex.Message } Details: { ex.ToString() }");
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
            catch(Exception e)
            {
                _logger.LogError($"Could not retrieve log file: {e}");
                return new BadRequestResult();
            }
        }




        [HttpPost]
        [Route("/ClientApp/Login")]
        public IActionResult AttemptLogin([FromBody] DeviceLoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation("POST /ClientApp/Login called");
                var data = _aquariumAuthService.AttemptLogin(loginRequest).Result;
                return new OkObjectResult(data.Account);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/ClientApp/Login caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("ClientApp/Logout")]
        public IActionResult Logout()
        {
            try
            {
                _logger.LogInformation("DELETE /ClientApp/Logout called");
                _aquariumAuthService.Logout();
                _scheduleService.StopAsync(_scheduleService.token).Wait();  //todo refactor this
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE /v1/ClientApp/Logout caught exception: { ex.Message } Details: { ex.ToString() }");
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
                _aquariumAuthService.RenewAuthenticationToken().Wait();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/ClientApp/Auth/Renew caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest();
            }
        }
    }
}
