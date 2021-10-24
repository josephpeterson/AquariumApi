using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AquariumApi.Models;
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
        [Route("/v1/WaterChange/ATO")]
        public IActionResult WaterChangeCompleteATO([FromBody] int maxRuntime)
        {
            try
            {
                _logger.LogInformation("POST /v1/WaterChange/ATO called");
                _atoService.BeginAutoTopOff(new AutoTopOffRequest
                {
                    RunIndefinitely = false,
                    Runtime = maxRuntime,
                });
                return new OkObjectResult(_atoService.GetATOStatus());
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/WaterChange/ATO endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return new BadRequestObjectResult(ex.Message);
            }
        }
        [HttpPost]
        [Route("/v1/WaterChange/ATO/Stop")]
        public IActionResult WaterChangeStopATO()
        {
            try
            {
                _logger.LogInformation("GET /v1/WaterChange/ATO/Stop called");
                _atoService.StopAutoTopOff();
                return new OkObjectResult(_atoService.GetATOStatus());
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/WaterChange/ATO/Stop endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/WaterChange/ATO/Status")]
        public IActionResult WaterChangeATOStatus()
        {
            try
            {
                _logger.LogInformation("GET /v1/WaterChange/ATO/Status called");
                return new OkObjectResult(_atoService.GetATOStatus());
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/WaterChange/ATO/Status endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/WaterChange/TestDeviceSensor")]
        public IActionResult TestDeviceSensor([FromBody] DeviceSensor deviceSensor)
        {
            try
            {
                _logger.LogInformation("GET /v1/WaterChange/TestDeviceSensor called");
                _gpioService.TestDeviceSensor(deviceSensor);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/WaterChange/TestDeviceSensor endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
    }
}
