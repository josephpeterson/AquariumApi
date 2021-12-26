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
    public class HomeController : Controller
    {
        private IDeviceService _deviceService;
        private ScheduleService _scheduleService;
        private IGpioService _gpioService;
        private IHardwareService _hardwareService;
        private IAquariumAuthService _aquariumAuthService;
        private IATOService _atoService;
        private ILogger<HomeController> _logger;
        private IConfiguration _config;
        private DeviceAPI _deviceAPI;

        public HomeController(IDeviceService deviceService,
            DeviceAPI deviceAPI,
            ScheduleService scheduleService,
            IHardwareService hardwareService,
            IGpioService gpioService,
            IATOService atoService,
            IAquariumAuthService aquariumAuthService,
            ILogger<HomeController> logger, IConfiguration config)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _gpioService = gpioService;
            _hardwareService = hardwareService;
            _aquariumAuthService = aquariumAuthService;
            _atoService = atoService;
            _logger = logger;
            _config = config;
            _deviceAPI = deviceAPI;
        }
        [HttpGet(DeviceOutboundEndpoints.PING)]
        public IActionResult PingDevice()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.PING} called");

                //todo maybe add authorization?? also maybe change how we do this process. the server should probably store a token
                //_aquariumAuthService.RenewAuthenticationToken().Wait(); //nvm dont do this, we dont want to trigger an update every time.

                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fileVersionInfo.FileVersion;
                string productVersion = fileVersionInfo.ProductVersion;

                var information = new DeviceInformation()
                {
                    Version = productVersion,
                    Aquarium = _aquariumAuthService.GetAquarium(),
                    Config = System.IO.File.ReadAllText("config.json"),
                    Schedules = _scheduleService.GetAllSchedules(),
                    Sensors = _gpioService.GetAllSensors(),
                    RunningJobs = _scheduleService.GetAllRunningJobs(),
                    ScheduledJobs = _scheduleService.GetAllScheduledTasks(),
                    UpdatedAt = DateTime.Now,
                    ATOStatus = null,
                };
                return new OkObjectResult(information);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.PING} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.PING} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        
        [HttpPost(DeviceOutboundEndpoints.UPDATE)]
        public IActionResult UpdateDeviceInformation([FromBody] AquariumDevice aquariumDevice)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.UPDATE} called");
                _aquariumAuthService.ApplyAquariumDeviceFromService(aquariumDevice);
                return Ok();
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.UPDATE} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.UPDATE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
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
