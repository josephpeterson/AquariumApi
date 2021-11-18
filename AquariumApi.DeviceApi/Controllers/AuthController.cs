using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [ApiController]
    [DisableCors]
    public class AuthController : Controller
    {
        private IDeviceService _deviceService;
        private ScheduleService _scheduleService;
        private IAquariumAuthService _aquariumAuthService;
        private ILogger<AuthController> _logger;
        private IConfiguration _config;

        public AuthController(IDeviceService deviceService,
            ScheduleService scheduleService,
            IAquariumAuthService aquariumAuthService,
            ILogger<AuthController> logger, IConfiguration config)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _aquariumAuthService = aquariumAuthService;
            _logger = logger;
            _config = config;
        }
        [HttpGet(DeviceOutboundEndpoints.AUTH_CURRENT)]
        public IActionResult GetDetailedInformation()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.AUTH_CURRENT} called");
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
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.AUTH_CURRENT} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.AUTH_CURRENT} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpPost(DeviceOutboundEndpoints.AUTH_LOGIN)]
        public IActionResult AttemptLogin([FromBody] DeviceLoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.AUTH_LOGIN} called");
                var data = _aquariumAuthService.AttemptLogin(loginRequest).Result;
                return new OkObjectResult(data.Account);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.AUTH_LOGIN} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.AUTH_LOGIN} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }

        [HttpDelete(DeviceOutboundEndpoints.AUTH_LOGOUT)]
        public IActionResult Logout()
        {
            try
            {
                _logger.LogInformation($"DELETE {DeviceOutboundEndpoints.AUTH_LOGOUT} called");
                _aquariumAuthService.Logout();
                _scheduleService.StopAsync(_scheduleService.token).Wait();  //todo refactor this
                return new OkResult();
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"DELETE {DeviceOutboundEndpoints.AUTH_LOGOUT} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE {DeviceOutboundEndpoints.AUTH_LOGOUT} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpGet(DeviceOutboundEndpoints.AUTH_RENEW)]
        public IActionResult RenewAuthenticationToken()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.AUTH_RENEW} called");
                _aquariumAuthService.RenewAuthenticationToken().Wait();
                return Ok();
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.AUTH_RENEW} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.AUTH_RENEW} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
    }
}
