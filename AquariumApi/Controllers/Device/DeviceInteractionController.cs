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
        public readonly IDeviceClient _deviceService;
        private readonly IAccountService _accountService;
        private readonly INotificationService _notificationService;
        public readonly ILogger<DeviceInteractionController> _logger;
        public DeviceInteractionController(IAquariumService aquariumService, IDeviceClient deviceService,
            IAccountService accountService,
            INotificationService notificationService,
            ILogger<DeviceInteractionController> logger)
        {
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _accountService = accountService;
            _notificationService = notificationService;
            _logger = logger;
        }


        [HttpPost]
        public IActionResult ApplyDeviceHardware([FromBody] AquariumDevice aquariumDevice)
        {
            try
            {
                _logger.LogInformation($"POST /v1/DeviceInteraction called");
                var userId = _accountService.GetCurrentUserId();
                var id = _accountService.GetCurrentAquariumId();
                var aquarium = _aquariumService.GetAquariumById(id);
                if (aquarium.Device == null)
                {
                    return BadRequest("This aquarium does not have a device");
                }
                if (!_accountService.CanModify(userId, aquarium))
                    return BadRequest("You do not own this aquarium");




                _logger.LogInformation("Attempting to determine local ip addresses");
                var localIp = Request.HttpContext.Connection.LocalIpAddress;
                var localPort = Request.HttpContext.Connection.LocalPort;
                var remoteIp = Request.HttpContext.Connection.RemoteIpAddress;
                var remotePort = Request.HttpContext.Connection.RemotePort;
                var aquariumPort = aquariumDevice.Port;

                _logger.LogInformation($"\n" +
                    $"- Local Ip Address: {localIp}:{localPort}" +
                    $"- Remote Ip Address: {remoteIp}:{remotePort}" +
                    $"- Determined Address: {remoteIp}:{aquariumPort}" +
                    $"\n");
                //Update IP Information
                var ip = remoteIp;
                var port = aquariumPort;
                if ($"{remoteIp}" == "::1")
                    ip = localIp;
                if (aquarium.Device.Address != $"{ip}" || aquarium.Device.Port != $"{port}")
                {
                    aquarium.Device.Address = $"{ip}";
                    aquarium.Device.Port = $"{port}";
                    aquarium.Device = _aquariumService.UpdateAquariumDevice(userId,aquarium.Device);
                    _notificationService.EmitAsync(userId, "Aquarium Device", $"[{aquarium.Device.Name}] Aquarium device ip/port combination was updated to ${ip}:{port}").Wait();
                }



                aquarium.Device = _aquariumService.ApplyAquariumDeviceHardware(aquarium.Device.Id, aquariumDevice);
                return new OkObjectResult(aquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/DeviceInteraction endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        /* 
         * Aquarium Device will attempt to retrieve its information upon boot.
         * Hardware information will be sent
         */
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

                aquarium.Device = _aquariumService.GetAquariumDeviceById(aquarium.Device.Id); //get detailed device
                return new OkObjectResult(aquarium.Device);
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

        //Device is sending us ATO dispatch
        [HttpPost]
        [Route("ATO")]
        public IActionResult RecieveATOStatus([FromBody] ATOStatus atoStatus)
        {
            try
            {
                _logger.LogInformation($"POST /v1/DeviceInteraction/ATO called");
                var userId = _accountService.GetCurrentUserId();
                var id = _accountService.GetCurrentAquariumId();
                var aquarium = _aquariumService.GetAquariumById(id);
                if (aquarium.Device == null)
                {
                    return BadRequest("This aquarium does not have a device");
                }
                if (!_accountService.CanModify(userId, aquarium))
                    return BadRequest("You do not own this aquarium");
                atoStatus.DeviceId = aquarium.Device.Id;
                atoStatus.AquariumId = aquarium.Id;

                var s = _aquariumService.UpdateDeviceATOStatus(atoStatus);

                if(s.Completed)
                    _logger.LogInformation($"ATO was completed for aquarium id: {id}");

                return new OkObjectResult(s);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/DeviceInteraction/ATO: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }


        //Device is asking for detailed device information
        [HttpGet]
        [Route("Device")]
        public IActionResult GetDeviceInformation()
        {
            try
            {
                _logger.LogInformation($"GET /v1/DeviceInteraction/Device called");
                var userId = _accountService.GetCurrentUserId();
                var id = _accountService.GetCurrentAquariumId();
                var aquarium = _aquariumService.GetAquariumById(id);
                if (aquarium.Device == null)
                {
                    return BadRequest("This aquarium does not have a device");
                }
                if (!_accountService.CanModify(userId, aquarium))
                    return BadRequest("You do not own this aquarium");

                return new OkObjectResult(_aquariumService.GetAquariumDeviceById(aquarium.Device.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/DeviceInteraction/Device endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

    }
    public class RequestModel
    {
        public IFormFile SnapshotImage { get; set; }
        public string Snapshot { get; set; }
    }
}