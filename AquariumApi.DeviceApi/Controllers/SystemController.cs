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
        [HttpGet(DeviceOutboundEndpoints.SYSTEM_LOG_RETRIEVE)]
        public IActionResult GetDeviceLog()
        {
            string txt;
            try
            {
                txt = System.IO.File.ReadAllText("AquariumDeviceApi.log");
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SYSTEM_LOG_RETRIEVE} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.SYSTEM_LOG_RETRIEVE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkObjectResult(txt);
        }
        [HttpPost(DeviceOutboundEndpoints.SYSTEM_LOG_CLEAR)]
        public IActionResult ClearDeviceLog()
        {
            try
            {
                System.IO.File.Delete("AquariumDeviceApi.log");
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SYSTEM_LOG_CLEAR} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SYSTEM_LOG_CLEAR} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkResult();
        }
        [HttpPost(DeviceOutboundEndpoints.SYSTEM_REBOOT)]
        public IActionResult AttemptReboot()
        {
            try
            {
                _deviceAPI.Process();
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SYSTEM_REBOOT} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SYSTEM_REBOOT} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkResult();
        }
        [HttpPost(DeviceOutboundEndpoints.SYSTEM_FACTORY_RESET)]
        public IActionResult PerformFactoryReset()
        {
            _deviceConfiguration.SaveDeviceConfiguration(new DeviceConfiguration());
            return new OkObjectResult(_deviceConfiguration.LoadDeviceConfiguration());
        }
        [HttpPost(DeviceOutboundEndpoints.UPDATE)]
        public IActionResult UpdateDeviceInformation([FromBody] DeviceConfiguration configuredDevice)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.UPDATE} called");
                _deviceConfiguration.SaveDeviceConfiguration(configuredDevice);
                return new OkObjectResult(_deviceConfiguration.LoadDeviceConfiguration());
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.UPDATE} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.UPDATE} endpoint caught exception: {ex.Message} Details: {ex.ToString()}");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }

    }
}
