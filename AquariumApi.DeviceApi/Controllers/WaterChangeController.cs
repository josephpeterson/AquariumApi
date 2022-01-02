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
        private IATOService _atoService;
        private IAquariumAuthService _aquariumAuthService;
        private ILogger<HomeController> _logger;
        private IConfiguration _config;

        public WaterChangeController(IDeviceService deviceService,
            ScheduleService scheduleService,
            IHardwareService hardwareService,
            IATOService atoService,
            IGpioService gpioService,
            IAquariumAuthService aquariumAuthService,
            ILogger<HomeController> logger, IConfiguration config)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _gpioService = gpioService;
            _hardwareService = hardwareService;
            _atoService = atoService;
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
                _atoService.BeginAutoTopOff(new AutoTopOffRequest
                {
                    RunIndefinitely = false,
                    Runtime = maxRuntime,
                });
                return new OkObjectResult(_atoService.GetATOStatus());
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
                _atoService.StopAutoTopOff();
                return new OkObjectResult(_atoService.GetATOStatus());
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
        [HttpGet]
        [Route(DeviceOutboundEndpoints.WATER_CHANGE_STATUS)]
        public IActionResult WaterChangeATOStatus()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.WATER_CHANGE_STATUS} called");
                return new OkObjectResult(_atoService.GetATOStatus());
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.WATER_CHANGE_STATUS} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.WATER_CHANGE_STATUS} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpPost]
        [Route(DeviceOutboundEndpoints.DEVICE_SENSOR_TEST)]
        public IActionResult TestDeviceSensor([FromBody] DeviceSensorTestRequest testRequest)
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.DEVICE_SENSOR_TEST} called");
                var finishedRequest = _gpioService.TestDeviceSensor(testRequest);
                return new OkObjectResult(finishedRequest);
            }
            catch(DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.DEVICE_SENSOR_TEST} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.DEVICE_SENSOR_TEST} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
    }
}
