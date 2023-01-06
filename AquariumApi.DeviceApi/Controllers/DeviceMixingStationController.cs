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
    public class DeviceMixingStationController : Controller
    {
        private ILogger<DeviceMixingStationController> _logger;
        private IConfiguration _config;
        private IWirelessDeviceService _wirelessDeviceService;

        public DeviceMixingStationController(
            IWirelessDeviceService mixingStationService,
            ILogger<DeviceMixingStationController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _wirelessDeviceService = mixingStationService;
        }
        [HttpPost(DeviceOutboundEndpoints.MIXING_STATION_PING)]
        public async Task<IActionResult> PingWirelessDevices([FromQuery] List<WirelessDevice> wirelessDevices)
        {
            try
            {
                var response = await _wirelessDeviceService.PingByHostname(wirelessDevices);
                return new OkObjectResult(response);
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.MIXING_STATION_PING} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpPost(DeviceOutboundEndpoints.MIXING_STATION_SEARCH)]
        public async Task<IActionResult> SearchForDevices()
        {
            try
            {
                var results = await _wirelessDeviceService.SearchForDevices();
                return new OkObjectResult(results);
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

        [HttpGet(DeviceOutboundEndpoints.MIXING_STATION_STATUS)]
        public async Task<IActionResult> GetWirelessDeviceStatuses()
        {
            try
            {
                var response = await _wirelessDeviceService.GetWirelessDeviceStatuses();
                return new OkObjectResult(response);
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.MIXING_STATION_PING} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkResult();
        }
        [HttpPost(DeviceOutboundEndpoints.MIXING_STATION_UPDATE)]
        public async Task<IActionResult> UpsertWirelessDevice([FromBody] WirelessDevice wirelessDevice)
        {
            try
            {
                var deviceConfiguration = _wirelessDeviceService.UpsertWirelessDevice(wirelessDevice);
                return new OkObjectResult(deviceConfiguration);
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.MIXING_STATION_UPDATE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpPost(DeviceOutboundEndpoints.MIXING_STATION_DELETE)]
        public IActionResult RemoveWirelessDevice([FromBody] WirelessDevice wirelessDevice)
        {
            try
            {
                var deviceConfiguration = _wirelessDeviceService.RemoveWirelessDevice(wirelessDevice);
                return new OkObjectResult(deviceConfiguration);
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.MIXING_STATION_PING} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpGet(DeviceOutboundEndpoints.MIXING_STATION_STOP)]
        public async Task<IActionResult> StopWirelessDevice(WirelessDevice wirelessDevice)
        {
            try
            {
                var wirelessDeviceStatus = await _wirelessDeviceService.StopWirelessDevice(wirelessDevice);
                return new OkObjectResult(wirelessDeviceStatus);
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.MIXING_STATION_STOP} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
    }
}
