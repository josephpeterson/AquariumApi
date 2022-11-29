using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [ApiController]
    public class SystemController : Controller
    {
        private ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private DeviceAPI _deviceAPI;
        private readonly IDeviceConfigurationService _deviceConfiguration;

        public SystemController(
            ILogger<HomeController> logger
            ,DeviceAPI deviceAPI
            ,IDeviceConfigurationService deviceConfiguration
            ,IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _deviceAPI = deviceAPI;
            _deviceConfiguration = deviceConfiguration;
        }
        [HttpGet(DeviceOutboundEndpoints.LOG)]
        public IActionResult GetDeviceLog()
        {
            string txt;
            try
            {
                txt = System.IO.File.ReadAllText("AquariumDeviceApi.log");
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.LOG} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.LOG} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkObjectResult(txt);
        }
        [HttpPost(DeviceOutboundEndpoints.LOG_CLEAR)]
        public IActionResult ClearDeviceLog()
        {
            try
            {
                System.IO.File.Delete("AquariumDeviceApi.log");
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.LOG_CLEAR} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.LOG_CLEAR} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkResult();
        }
        [HttpPost(DeviceOutboundEndpoints.REBOOT)]
        public IActionResult AttemptReboot()
        {
            try
            {
                _deviceAPI.Process();
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.REBOOT} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.REBOOT} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkResult();
        }

    }
}
