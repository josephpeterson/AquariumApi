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

        [HttpGet]
        [Route("/v1/WaterChange/Drain")]
        public IActionResult WaterChangeDrain()
        {
            try
            {
                _logger.LogInformation("GET /v1/WaterChange/Drain called");
                _gpioService.WaterChangeBeginDrain();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/WaterChange/Drain endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/WaterChange/Sensors")]
        public IActionResult WaterChangeGetSensorValues()
        {
            try
            {
                _logger.LogInformation("GET /v1/WaterChange/Sensors called");
                var val = _gpioService.GetSensorValues();
                return new OkObjectResult(val);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/WaterChange/Sensors endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/WaterChange/DrainTest")]
        public IActionResult WaterChangeTestDrain(int ms)
        {
            try
            {
                _logger.LogInformation("GET /v1/WaterChange/DrainTest called");
                _gpioService.WaterChangeDrainTest(ms);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/WaterChange/DrainTest endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
    }
}
