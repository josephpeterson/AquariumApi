using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AquariumApi.Controllers
{
    [Authorize(Roles = "Device")]
    [Route("/v1/DeviceInteraction")]
    public class DeviceInteractionController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly IDeviceService _deviceService;
        private readonly IAccountService _accountService;
        public readonly ILogger<DeviceInteractionController> _logger;
        public DeviceInteractionController(IAquariumService aquariumService, IDeviceService deviceService,
            IAccountService accountService,
            ILogger<DeviceInteractionController> logger)
        {
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _accountService = accountService;
            _logger = logger;
        }
        [HttpPost]
        public IActionResult GetDeviceById([FromBody] AquariumDevice aquariumDevice)
        {
            try
            {
                _logger.LogInformation($"POST /v1/DeviceInteraction called");
                var userId = _accountService.GetCurrentUserId();
                var id = _accountService.GetCurrentAquariumId();
                var aquarium = _aquariumService.GetAquariumById(id);
                if(aquarium.Device == null)
                {
                    return BadRequest("This aquarium does not have a device");
                }
                if(!_accountService.CanModify(userId,aquarium))
                    return BadRequest("You do not own this aquarium");

                aquarium.Device = _aquariumService.ApplyAquariumDeviceHardware(aquarium.Device.Id, aquariumDevice);
                return new OkObjectResult(aquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/DeviceInteraction endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        public IActionResult RecievePing()
        {
            try
            {
                _logger.LogInformation($"GET /v1/DeviceInteraction called");
                var userId = _accountService.GetCurrentUserId();
                var id = _accountService.GetCurrentAquariumId();
                var aquarium = _aquariumService.GetAquariumById(id);
                if (aquarium.Device == null)
                {
                    return BadRequest("This aquarium does not have a device");
                }
                if (!_accountService.CanModify(userId, aquarium))
                    return BadRequest("You do not own this aquarium");
                return new OkObjectResult(new DeviceLoginResponse
                {
                    Account = _aquariumService.GetAccountDetailed(userId, userId),
                    Aquarium = aquarium,
                    AquariumId = id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/DeviceInteraction endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

        //Recieve snapshot from device
        [HttpPost, DisableRequestSizeLimit]
        [Route("Snapshot")]
        public IActionResult RecieveSnapshot(RequestModel data)
        {
            try
            {
                _logger.LogInformation($"POST /v1/DeviceInteraction/Snapshot called");
                var snapshotImage = data.SnapshotImage;
                var snapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(data.Snapshot);
                var userId = _accountService.GetCurrentUserId();
                var id = _accountService.GetCurrentAquariumId();
                var aquarium = _aquariumService.GetAquariumById(id);
                if (!_accountService.CanModify(userId, aquarium))
                    return BadRequest("You do not own this aquarium");
                AquariumSnapshot s = _aquariumService.AddSnapshot(id, snapshot, snapshotImage);
                return new OkObjectResult(s);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/DeviceInteraction/Snapshot: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

    }
    public class RequestModel
    {
        public IFormFile SnapshotImage { get; set; }
        public string Snapshot { get; set; }
    }
}