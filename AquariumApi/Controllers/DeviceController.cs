using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class DeviceController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly IDeviceService _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceController(IAquariumService aquariumService, IDeviceService deviceService,ILogger<DeviceController> logger)
        {
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _logger = logger;
        }
        [HttpGet]
        [Route("/v1/Device/{deviceId}")]
        public IActionResult GetDeviceById(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId} called");
                var device = _aquariumService.GetAquariumDeviceById(deviceId);
                return new OkObjectResult(device);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Device/{deviceId}/Delete")]
        public IActionResult DeleteAquariumDevice(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Delete called");
                _aquariumService.DeleteAquariumDevice(deviceId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Device/Update")]
        public IActionResult UpdateAquariumDevice([FromBody] AquariumDevice device)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/Update called");
                var updatedDevice = _aquariumService.UpdateAquariumDevice(device);
                return new OkObjectResult(updatedDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/Update endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Device/Add")]
        public IActionResult AddAquariumDevice([FromBody] AquariumDevice device)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/Add called");
                var newDevice = _aquariumService.AddAquariumDevice(device);
                return CreatedAtAction(nameof(UpdateAquariumDevice), new { id = newDevice.Id }, newDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/Add endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Device/{deviceId}/Scan")]
        public IActionResult ScanAquariumDeviceHardware(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Scan called");
                var updatedDevice = _deviceService.ScanHardware(deviceId);
                var deviceToUpdate = _aquariumService.ApplyAquariumDeviceHardware(deviceId,updatedDevice);
                return new OkObjectResult(deviceToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Update endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("/v1/Device/{deviceId}/Ping")]
        public IActionResult PingAquariumDevice(int deviceId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Device/{deviceId}/Ping called");
                bool pong = _deviceService.Ping(deviceId);
                if (pong)
                    return new OkResult();
                else
                    return new NotFoundResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Device/{deviceId}/Ping endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

        [HttpPost]
        [Route("/v1/Device/{deviceId}/CameraConfiguration")]
        public IActionResult UpdateCameraConfiguration(int deviceId,[FromBody] CameraConfiguration config)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/CameraConfiguration called");
                var aquariumDevice = _aquariumService.UpdateDeviceCameraConfiguration(config);
                return new OkObjectResult(aquariumDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/CameraConfiguration endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }


        //Ping recieved
        [HttpPost]
        [Route("/v1/Device/Ping")]
        public IActionResult GetPingFromDevice([FromBody] AquariumDevice deviceRequest)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Device/Ping called");

                var host = HttpContext.Connection.RemoteIpAddress.ToString();

                _logger.LogInformation($"\n\nHost: ({host}) \n '{deviceRequest.PrivateKey}'");

                var device = _aquariumService.GetAquariumDeviceByIpAndKey(host, deviceRequest.PrivateKey);
                _aquariumService.ApplyAquariumDeviceHardware(device.Id, deviceRequest);
                return new OkObjectResult(device);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Device/Ping endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("/v1/Device/Ping")]
        public IActionResult sdasdas()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Device/Ping called");

                var host = HttpContext.Connection.RemoteIpAddress;
                return new OkObjectResult(host.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Device/Ping endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }


        //Recieve snapshot from device
        [HttpPost, DisableRequestSizeLimit]
        [Route("/v1/Device/{deviceId}/Snapshot")]
        public IActionResult UploadSnapshot(int deviceId,IFormFile snapshotImage,AquariumSnapshot snapshot)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Snapshot called");
                _logger.LogInformation($"Test: {snapshot.Date.ToString()}");
                var device = _aquariumService.GetAquariumDeviceById(deviceId);
                AquariumSnapshot s = _aquariumService.AddSnapshot(device.AquariumId, snapshot,snapshotImage);
                return new OkObjectResult(s);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Snapshot: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        //Retrieve AquariumDeviceApi.log
        [HttpPost, DisableRequestSizeLimit]
        [Route("/v1/Device/{deviceId}/Log")]
        public IActionResult ClearDeviceLog(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Log called");
                var deviceLog = _deviceService.GetDeviceLog(deviceId);
                return new OkObjectResult(deviceLog);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Log: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost, DisableRequestSizeLimit]
        [Route("/v1/Device/{deviceId}/Log/Clear")]
        public IActionResult GetDeviceLog(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Log/Clear called");
                _deviceService.ClearDeviceLog(deviceId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Log/Clear: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        //Retrieve Assigned aquarium/schedule information
        [HttpPost, DisableRequestSizeLimit]
        [Route("/v1/Device/{deviceId}/Information")]
        public IActionResult GetDeviceInformation(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Information called");
                var deviceInformation = _deviceService.GetDeviceInformation(deviceId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Information: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }


        //Retrieve Assigned aquarium/schedule information
        [HttpGet]
        [Route("/v1/Device/{deviceId}/UpdateConfigurationFile")]
        public IActionResult UpdateConfigurationFile(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Information called");
                var deviceInformation = _deviceService.GetDeviceInformation(deviceId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Information: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }




        //Deploy/Remove Device Schedules
        [HttpPost]
        [Route("/v1/Device/{deviceId}/DeploySchedule/{scheduleId}")]
        public IActionResult DeploySchedule(int deviceId,int scheduleId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/DeploySchedule/{scheduleId} called");
                var scheduleAssignment = _aquariumService.DeployDeviceSchedule(deviceId, scheduleId);
                return new OkObjectResult(scheduleAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/DeploySchedule/{scheduleId}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Device/{deviceId}/RemoveSchedule/{scheduleId}")]
        public IActionResult RemoveSchedule(int deviceId, int scheduleId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/RemoveSchedule/{scheduleId} called");
                var scheduleAssignment = _aquariumService.RemoveDeviceSchedule(deviceId, scheduleId);
                return new OkObjectResult(scheduleAssignment);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/RemoveSchedule/{scheduleId}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Device/{deviceId}/Schedule/Status")]
        public IActionResult GetScheduleStatus(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Schedule/Status called");
                ScheduleState scheduleState = _deviceService.GetDeviceScheduleStatus(deviceId);
                return new OkObjectResult(scheduleState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Schedule/Status: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

    }
}