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
        private ScheduleService _scheduleService;
        private ILogger<HomeController> _logger;
        private IConfiguration _config;
        private readonly IDeviceConfigurationService _deviceConfiguration;

        public HomeController(
            ScheduleService scheduleService,
            IDeviceConfigurationService deviceConfiguration,
            ILogger<HomeController> logger, IConfiguration config)
        {
            _scheduleService = scheduleService;
            _logger = logger;
            _config = config;
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
                    Account = _deviceConfiguration.LoadAccountInformation()?.Account,
                    ScheduleStatus = _scheduleService.GetScheduleStatus()
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
        
        [HttpGet(DeviceOutboundEndpoints.SELECT_FORM_TYPES)]
        public IActionResult GetTypeSelectOptions(string selectType)
        {
            try
            {
                List<KeyValuePair<string, int>> options;
                switch (selectType)
                {
                    case "DeviceSensorTypes":
                        options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Other",(int)SensorTypes.Other),
                        new KeyValuePair<string, int>("GPIO Sensor",(int)SensorTypes.Sensor),
                        new KeyValuePair<string, int>("Mixing Station",(int)SensorTypes.MixingStation),
                    };
                        break;
                    case "DeviceSensorValues":
                        options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Low",(int)GpioPinValue.Low),
                        new KeyValuePair<string, int>("High",(int)GpioPinValue.High),
                    };
                        break;
                    case "DeviceTaskTypes":
                        options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Take Snapshot",(int)ScheduleTaskTypes.Snapshot),
                        new KeyValuePair<string, int>("Start ATO",(int)ScheduleTaskTypes.StartATO),
                        new KeyValuePair<string, int>("Water Change Drain",(int)ScheduleTaskTypes.WaterChangeDrain),
                        new KeyValuePair<string, int>("Water Change Replentish",(int)ScheduleTaskTypes.WaterChangeReplentish),
                        new KeyValuePair<string, int>("Unknown",(int)ScheduleTaskTypes.Unknown),
                    };
                        break;
                    case "TriggerTypes":
                        options = new List<KeyValuePair<string, int>>()
                    {
                        new KeyValuePair<string, int>("Start at Time",(int)TriggerTypes.Time),
                        new KeyValuePair<string, int>("Sensor Condition",(int)TriggerTypes.SensorDependent),
                        new KeyValuePair<string, int>("Task Condition",(int)TriggerTypes.TaskDependent),
                        new KeyValuePair<string, int>("Task Assignment",(int)TriggerTypes.TaskAssignmentCompleted),
                    };
                        break;
                    default:
                        options = new List<KeyValuePair<string, int>>();
                        break;
                }
                return new OkObjectResult(options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /Form/Select/{selectType} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}
