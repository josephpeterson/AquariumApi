using System;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [ApiController]
    public class WaterChangeController : Controller
    {
        private IDeviceService _deviceService;
        private ScheduleService _scheduleService;
        private IGpioService _gpioService;
        private IHardwareService _hardwareService;
        private IAquariumAuthService _aquariumAuthService;
        private ILogger<HomeController> _logger;
        private IConfiguration _config;

        public WaterChangeController(IDeviceService deviceService,
            ScheduleService scheduleService,
            IHardwareService hardwareService,
            IGpioService gpioService,
            IAquariumAuthService aquariumAuthService,
            ILogger<HomeController> logger, IConfiguration config)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _gpioService = gpioService;
            _hardwareService = hardwareService;
            _aquariumAuthService = aquariumAuthService;
            _logger = logger;
            _config = config;
        }

        [HttpPost]
        [Route(DeviceOutboundEndpoints.WATER_CHANGE_BEGIN)]
        public IActionResult WaterChangeCompleteATO([FromBody] int maxRuntime)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.WATER_CHANGE_BEGIN} called");
                //basically look for the tasks to trigger water change if its configured
                throw new NotImplementedException();
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.WATER_CHANGE_BEGIN} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.WATER_CHANGE_BEGIN} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpPost]
        [Route(DeviceOutboundEndpoints.WATER_CHANGE_STOP)]
        public IActionResult WaterChangeStopATO()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.WATER_CHANGE_STOP} endpoint called");
                throw new NotImplementedException();
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.WATER_CHANGE_STOP} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.WATER_CHANGE_STOP} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        
    }
}
