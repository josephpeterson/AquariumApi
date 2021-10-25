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
    public class HomeController : Controller
    {
        private IDeviceService _deviceService;
        private ScheduleService _scheduleService;
        private IGpioService _gpioService;
        private IHardwareService _hardwareService;
        private IAquariumAuthService _aquariumAuthService;
        private ILogger<HomeController> _logger;
        private IConfiguration _config;

        public HomeController(IDeviceService deviceService,
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
        [Route("/v1/Scan")]
        public IActionResult ScanHardware()
        {
            try
            {
                _logger.LogInformation("GET /v1/Scan called");
                var hardware = _hardwareService.ScanHardware();
                return new OkObjectResult(hardware);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Scan endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Ping")]
        public IActionResult Ping()
        {
            try
            {
                _logger.LogInformation("POST /v1/Ping called");
                //_aquariumAuthService.SaveTokenToCache(loginResponse);
                //_deviceService.ReloadAuthenticationToken();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Ping endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Information")]
        public IActionResult GetDeviceInformation()
        {
            try
            {
                _logger.LogInformation("POST /v1/Information called");
                var information = new DeviceInformation()
                {
                    Aquarium = _aquariumAuthService.GetAquarium(),
                    config = System.IO.File.ReadAllText("config.json"),
                    Schedules = _scheduleService.GetAllSchedules(),
                    Sensors = _gpioService.GetAllSensors()
                };
                return new OkObjectResult(information);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Information endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Device")]
        public IActionResult UpdateDeviceInformation([FromBody] AquariumDevice aquariumDevice)
        {
            try
            {
                _logger.LogInformation("POST /v1/Device called");
                _aquariumAuthService.ApplyAquariumDeviceFromService(aquariumDevice);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }

    }
}
