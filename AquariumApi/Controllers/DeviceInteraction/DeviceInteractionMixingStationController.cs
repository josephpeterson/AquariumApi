using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AquariumApi.Controllers.DeviceInteraction
{
    [Authorize]
    [Route("DeviceInteraction")]
    public class DeviceInteractionMixingStationController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAquariumService _aquariumService;
        private readonly IAquariumDeviceInteractionService _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceInteractionMixingStationController(IAccountService accountService
            ,IAquariumService aquariumService
            ,IAquariumDeviceInteractionService deviceService
            ,ILogger<DeviceController> logger)
        {
            _accountService = accountService;
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _logger = logger;
        }
        private bool ValidateRequest(int aquariumId)
        {
            var id = _accountService.GetCurrentUserId();
            var aquariums = _aquariumService.GetAquariumsByAccountId(id);
            return aquariums.Where(a => a.Id == aquariumId && a.Device != null).Any();
        }
        [HttpPost("{aquariumId}/" + DeviceOutboundEndpoints.MIXING_STATION_PING)]
        public async Task<IActionResult> PingMixingStationByHostname(int aquariumId,[FromQuery] string hostname)
        {
            throw new NotImplementedException();
        }
        [HttpPost("{aquariumId}/" + DeviceOutboundEndpoints.MIXING_STATION_SEARCH)]
        public async Task<IActionResult> SearchForMixingStation(int aquariumId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{aquariumId}/" + DeviceOutboundEndpoints.MIXING_STATION_STATUS)]
        public async Task<IActionResult> PingMixingStation(int aquariumId)
        {
            if (!ValidateRequest(aquariumId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST DeviceInteraction/{DeviceOutboundEndpoints.MIXING_STATION_STATUS} called");
                var deviceSensors = await _deviceService.GetMixingStationStatus(aquariumId);
                return new OkObjectResult(deviceSensors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.MIXING_STATION_STATUS}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost("{aquariumId}/" + DeviceOutboundEndpoints.MIXING_STATION_UPDATE)]
        public async Task<IActionResult> UpsertMixingStation(int aquariumId, [FromBody] AquariumMixingStationStatus mixingStation)
        {
            throw new NotImplementedException();
        }
        [HttpDelete("{aquariumId}/" + DeviceOutboundEndpoints.MIXING_STATION_DELETE)]
        public IActionResult DisconnectMixingStation(int aquariumId)
        {
            throw new NotImplementedException();
        }
        [HttpGet("{aquariumId}/" + DeviceOutboundEndpoints.MIXING_STATION_TEST_VALVE)]
        public async Task<IActionResult> TestMixingStationValve(int aquariumId, int valveId)
        {
            throw new NotImplementedException();
        }
        [HttpGet("{aquariumId}/" + DeviceOutboundEndpoints.MIXING_STATION_STOP)]
        public async Task<IActionResult> StopMixingStationSessions(int aquariumId, int valveId)
        {
            throw new NotImplementedException();
        }
    }
}