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
        private IMixingStationService _mixingStationService;

        public DeviceMixingStationController(
            IMixingStationService mixingStationService,
            ILogger<DeviceMixingStationController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _mixingStationService = mixingStationService;
        }
        [HttpPost(DeviceOutboundEndpoints.MIXING_STATION_PING)]
        public async Task<IActionResult> PingMixingStationByHostname([FromQuery] string hostname)
        {
            try
            {
                var response = await _mixingStationService.PingByHostname(hostname);
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
        [HttpPost(DeviceOutboundEndpoints.MIXING_STATION_SEARCH)]
        public async Task<IActionResult> SearchForMixingStation()
        {
            try
            {
                var results = await _mixingStationService.SearchForMixingStation();
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
        public async Task<IActionResult> PingMixingStation()
        {
            try
            {
                var response = await _mixingStationService.PingByHostname();
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
        public async Task<IActionResult> UpsertMixingStation([FromBody] AquariumMixingStationStatus mixingStation)
        {
            try
            {
                var config = await _mixingStationService.ConnectToMixingStation(mixingStation);
                return new OkObjectResult(config);
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
        [HttpDelete(DeviceOutboundEndpoints.MIXING_STATION_DELETE)]
        public IActionResult DisconnectMixingStation()
        {
            try
            {
                _mixingStationService.DisconnectFromMixingStation();
                return new OkResult();
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
        [HttpGet(DeviceOutboundEndpoints.MIXING_STATION_TEST_VALVE)]
        public async Task<IActionResult> TestMixingStationValve(int valveId)
        {
            try
            {
                var response = await _mixingStationService.TestMixingStationValve(valveId);
                return new OkObjectResult(response);
            }
            catch (DeviceException ex)
            {
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.MIXING_STATION_TEST_VALVE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
            return new OkResult();
        }
        [HttpGet(DeviceOutboundEndpoints.MIXING_STATION_STOP)]
        public async Task<IActionResult> StopMixingStationSessions(int valveId)
        {
            try
            {
                var response = await _mixingStationService.StopMixingStationSessions();
                return new OkObjectResult(response);
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
