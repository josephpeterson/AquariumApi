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
        private ILogger<HomeController> _logger;
        private IConfiguration _config;
        private DeviceAPI _deviceAPI;
        private IMixingStationService _mixingStationService;
        private readonly IDeviceConfigurationService _deviceConfiguration;

        public HomeController(IDeviceService deviceService,
            DeviceAPI deviceAPI,
            ScheduleService scheduleService,
            IHardwareService hardwareService,
            IGpioService gpioService,
            IAquariumAuthService aquariumAuthService,
            IMixingStationService mixingStationService,
            IDeviceConfigurationService deviceConfiguration,
            ILogger<HomeController> logger, IConfiguration config)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _gpioService = gpioService;
            _hardwareService = hardwareService;
            _aquariumAuthService = aquariumAuthService;
            _logger = logger;
            _config = config;
            _deviceAPI = deviceAPI;
            _mixingStationService = mixingStationService;
            _deviceConfiguration = deviceConfiguration;
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
                    Config = System.IO.File.ReadAllText("config.json"),
                    ConfiguredDevice = _deviceConfiguration.LoadDeviceConfiguration(),
                    RunningJobs = _scheduleService.GetAllRunningJobs(),
                    ScheduledJobs = _scheduleService.GetAllScheduledTasks(),
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
        public IActionResult UpdateDeviceInformation([FromBody] Aquarium assignedAquarium)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.UPDATE} called");
                _aquariumAuthService.ApplyAquariumDeviceFromService(assignedAquarium);
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
        [HttpPost(DeviceOutboundEndpoints.MIXING_STATION_SEARCH)]
        public async Task<IActionResult> SearchForMixingStationByHostname([FromQuery] string hostname)
        {
            try
            {
                var response = await _mixingStationService.SearchByHostname(hostname);
                return new OkObjectResult(response);
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.MIXING_STATION_SEARCH} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkResult();
        }
        [HttpGet(DeviceOutboundEndpoints.MIXING_STATION_STATUS)]
        public async Task<IActionResult> GetMixingStation()
        {
            try
            {
                var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();

                if (deviceConfiguration.MixingStation == null || string.IsNullOrEmpty(deviceConfiguration.MixingStation.Hostname))
                    return BadRequest("No mixing station connected.");
                var response = await _mixingStationService.SearchByHostname(deviceConfiguration.MixingStation.Hostname);
                return new OkObjectResult(response);
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.MIXING_STATION_SEARCH} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkResult();
        }
        [HttpPost(DeviceOutboundEndpoints.MIXING_STATION_UPDATE)]
        public async Task<IActionResult> UpsertMixingStation([FromBody] AquariumMixingStation mixingStation)
        {
            try
            {
                var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
                deviceConfiguration.MixingStation = mixingStation;
                _deviceConfiguration.SaveDeviceConfiguration(deviceConfiguration);
                return new OkObjectResult(deviceConfiguration);
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.MIXING_STATION_SEARCH} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkResult();
        }
        [HttpDelete(DeviceOutboundEndpoints.MIXING_STATION_DELETE)]
        public IActionResult DisconnectMixingStation()
        {
            try
            {
                var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
                deviceConfiguration.MixingStation = null;
                _deviceConfiguration.SaveDeviceConfiguration(deviceConfiguration);
                return new OkResult();
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.MIXING_STATION_SEARCH} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
    }
}
