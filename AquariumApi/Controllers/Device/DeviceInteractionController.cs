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


        //todo review this
        [HttpPost]
        [Route(DeviceInboundEndpoints.CONNECT_DEVICE)]
        public IActionResult ApplyDeviceHardware([FromBody] AquariumDevice aquariumDevice)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceInboundEndpoints.CONNECT_DEVICE} called");
                if (!ValidateRequest())
                    return Unauthorized();

                var aquarium = GetCurrentAquarium();
                var userId = _accountService.GetCurrentUserId();
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
                _logger.LogError($"POST {DeviceInboundEndpoints.CONNECT_DEVICE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        /* 
         * Aquarium Device will attempt to retrieve its information upon boot.
         * Hardware information will be sent
         */
        [HttpGet]
        [Route(DeviceInboundEndpoints.RECEIVE_PING)]
        public IActionResult RecievePing()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceInboundEndpoints.RECEIVE_PING} called");
                if (!ValidateRequest())
                    return Unauthorized();

                var aquarium = GetCurrentAquarium();
                aquarium.Device = _aquariumService.GetAquariumDeviceById(aquarium.Device.Id); //get detailed device
                return new OkObjectResult(aquarium.Device);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceInboundEndpoints.RECEIVE_PING} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

        //Recieve snapshot from device
        [HttpPost, DisableRequestSizeLimit]
        [Route(DeviceInboundEndpoints.DISPATCH_SNAPSHOT)]
        public IActionResult RecieveSnapshot(RequestModel data)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceInboundEndpoints.DISPATCH_SNAPSHOT} called");
                if (!ValidateRequest())
                    return Unauthorized();

                var aquarium = GetCurrentAquarium();
                var snapshotImage = data.SnapshotImage;
                var snapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(data.Snapshot);
                AquariumSnapshot s = _aquariumService.AddSnapshot(aquarium.Id, snapshot, snapshotImage);
                return new OkObjectResult(s);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceInboundEndpoints.DISPATCH_SNAPSHOT}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        //Device is sending us ATO dispatch
        [HttpPost]
        [Route(DeviceInboundEndpoints.DISPATCH_ATO)]
        public IActionResult RecieveATOStatus([FromBody] ATOStatus atoStatus)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceInboundEndpoints.DISPATCH_ATO} called");
                if (!ValidateRequest())
                    return Unauthorized();

                var aquarium = GetCurrentAquarium();
                atoStatus.DeviceId = aquarium.Device.Id;
                atoStatus.AquariumId = aquarium.Id;

                var s = _aquariumService.UpdateDeviceATOStatus(atoStatus);

                if(s.Completed)
                    _logger.LogInformation($"ATO was completed for aquarium id: {aquarium.Id}");

                return new OkObjectResult(s);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceInboundEndpoints.DISPATCH_ATO}: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }


        //Device is asking for detailed device information
        [HttpGet]
        [Route(DeviceInboundEndpoints.RETRIEVE_DEVICE)]
        public IActionResult GetDeviceInformation()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceInboundEndpoints.RETRIEVE_DEVICE} called");
                if (!ValidateRequest())
                    return Unauthorized();

                var aquarium = GetCurrentAquarium();
                return new OkObjectResult(_aquariumService.GetAquariumDeviceById(aquarium.Device.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceInboundEndpoints.RETRIEVE_DEVICE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

        //Device is sending us a dispatch on a scheduled job
        [HttpPut]
        [Route(DeviceInboundEndpoints.DISPATCH_SCHEDULEDJOB)]
        public IActionResult UpsertScheduledJob([FromBody] ScheduledJob scheduledJob)
        {
            try
            {
                if (!ValidateRequest())
                    return Unauthorized();

                var aquarium = GetCurrentAquarium();
                scheduledJob.DeviceId = aquarium.Device.Id;
                _logger.LogInformation($"PUT {DeviceInboundEndpoints.DISPATCH_SCHEDULEDJOB} called");
                ScheduledJob updatedScheduledJob = _aquariumService.UpsertDeviceScheduledJob(scheduledJob);
                return new OkObjectResult(updatedScheduledJob);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceInboundEndpoints.DISPATCH_SCHEDULEDJOB} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        private bool ValidateRequest()
        {
            var userId = _accountService.GetCurrentUserId();
            var id = _accountService.GetCurrentAquariumId();
            var aquarium = _aquariumService.GetAquariumById(id);
            if (aquarium.Device == null)
                return false;
            if (!_accountService.CanModify(userId, aquarium))
                return false;
            return true;
        }
        private Aquarium GetCurrentAquarium()
        {
            var id = _accountService.GetCurrentAquariumId();
            var aquarium = _aquariumService.GetAquariumById(id);
            return aquarium;
        }
    }
    public class RequestModel
    {
        public IFormFile SnapshotImage { get; set; }
        public string Snapshot { get; set; }
    }
}