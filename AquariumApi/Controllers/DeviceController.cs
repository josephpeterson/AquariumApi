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
    [Authorize]
    public class DeviceController : Controller
    {
        private readonly IAccountService _accountService;
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceController(IAccountService accountService,IAquariumService aquariumService,ILogger<DeviceController> logger)
        {
            _accountService = accountService;
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_RETRIEVE)]
        public IActionResult GetDeviceById(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_RETRIEVE.AggregateParams($"{deviceId}")} called");
                var device = _aquariumService.GetAquariumDeviceById(deviceId);
                return new OkObjectResult(device);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_RETRIEVE.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_DELETE)]
        public IActionResult DeleteAquariumDevice(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST  {AquariumApiEndpoints.DEVICE_DELETE.AggregateParams($"{deviceId}")} called");
                _aquariumService.DeleteAquariumDevice(deviceId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST  {AquariumApiEndpoints.DEVICE_DELETE.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_UPDATE)]
        public IActionResult UpdateAquariumDevice([FromBody] AquariumDevice device)
        {
            if (!ValidateRequest(device.Id))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_UPDATE} called");
                var id = _accountService.GetCurrentUserId();
                var aq = _aquariumService.GetAquariumById(device.AquariumId);
                if(!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var updatedDevice = _aquariumService.UpdateAquariumDevice(id,device);
                return new OkObjectResult(updatedDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_UPDATE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_CREATE)]
        public IActionResult AddAquariumDevice([FromBody] AquariumDevice device)
        {
            //todo validate
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_CREATE} called");
                var newDevice = _aquariumService.AddAquariumDevice(device);
                return CreatedAtAction(nameof(UpdateAquariumDevice), new { id = newDevice.Id }, newDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_CREATE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_DISPATCH_SCAN)]
        public IActionResult ScanAquariumDeviceHardware(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_SCAN.AggregateParams($"{deviceId}")} called");
                var updatedDevice = _aquariumService.ScanHardware(deviceId);
                var deviceToUpdate = _aquariumService.ApplyAquariumDeviceHardware(deviceId,updatedDevice);
                return new OkObjectResult(deviceToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_SCAN.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_DISPATCH_PING)]
        public IActionResult PingAquariumDevice(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {

                _logger.LogInformation($"GET {AquariumApiEndpoints.DEVICE_DISPATCH_PING.AggregateParams($"{deviceId}")} called");
                var deviceInformation = _aquariumService.PingDevice(deviceId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {AquariumApiEndpoints.DEVICE_DISPATCH_PING.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_DISPATCH_AUTH_RENEW)]
        public IActionResult AttemptAuthRenew(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {

                _logger.LogInformation($"GET {AquariumApiEndpoints.DEVICE_DISPATCH_AUTH_RENEW.AggregateParams($"{deviceId}")} called");
                _aquariumService.AttemptAuthRenewDevice(deviceId);
                return new OkObjectResult(true);
            }
            catch
            {
                return new OkObjectResult(false);
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.DEVICE_DISPATCH_SNAPSHOT_CONFIGURATION)]
        public IActionResult UpdateCameraConfiguration(int deviceId,[FromBody] CameraConfiguration config)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_SNAPSHOT_CONFIGURATION.AggregateParams($"{deviceId}")} called");
                var aquariumDevice = _aquariumService.UpdateDeviceCameraConfiguration(config);
                return new OkObjectResult(aquariumDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_DISPATCH_SNAPSHOT_CONFIGURATION.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

       //Retrieve AquariumDeviceApi.log
        [HttpPost, DisableRequestSizeLimit]
        [Route(AquariumApiEndpoints.DEVICE_LOG)]
        public IActionResult GetDeviceLog(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_LOG.AggregateParams($"{deviceId}")} called");
                var deviceLog = _aquariumService.GetDeviceLog(deviceId);
                return new OkObjectResult(deviceLog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_LOG.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        
        [HttpPost, DisableRequestSizeLimit]
        [Route(AquariumApiEndpoints.DEVICE_LOG_CLEAR)]
        public IActionResult ClearDeviceLog(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_LOG_CLEAR.AggregateParams($"{deviceId}")} called");
                _aquariumService.ClearDeviceLog(deviceId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_LOG_CLEAR.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        
        //Retrieve Assigned aquarium/schedule information
        [HttpPost, DisableRequestSizeLimit]
        [Route(AquariumApiEndpoints.DEVICE_RETRIEVE_DETAILED)]
        public IActionResult GetDeviceInformation(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_RETRIEVE_DETAILED.AggregateParams($"{deviceId}")} called");
                var deviceInformation = _aquariumService.GetDeviceInformation(deviceId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_RETRIEVE_DETAILED.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        //Retrieve Assigned aquarium/schedule information
        [HttpGet]
        [Route(AquariumApiEndpoints.DEVICE_UPDATE_CONFIGURATION)]
        public IActionResult UpdateConfigurationFile(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.DEVICE_UPDATE_CONFIGURATION.AggregateParams($"{deviceId}")} called");
                var deviceInformation = _aquariumService.GetDeviceInformation(deviceId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.DEVICE_UPDATE_CONFIGURATION.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        private bool ValidateRequest(int deviceId)
        {
            var id = _accountService.GetCurrentUserId();
            var aquariums = _aquariumService.GetAquariumsByAccountId(id);
            return aquariums.Where(a => a.Device.Id == deviceId).Any();
        }
    }
}