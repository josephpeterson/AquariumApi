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
                var id = _accountService.GetCurrentUserId();
                var aq = _aquariumService.GetAquariumById(device.AquariumId);
                if(!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var updatedDevice = _aquariumService.UpdateAquariumDevice(id,device);
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
                var updatedDevice = _aquariumService.ScanHardware(deviceId);
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
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {

                _logger.LogInformation($"GET /v1/Device/{deviceId}/Ping called");
                 if(_aquariumService.Ping(deviceId))
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


       //Retrieve AquariumDeviceApi.log
        [HttpPost, DisableRequestSizeLimit]
        [Route("/v1/Device/{deviceId}/Log")]
        public IActionResult ClearDeviceLog(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Log called");
                var deviceLog = _aquariumService.GetDeviceLog(deviceId);
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
                _aquariumService.ClearDeviceLog(deviceId);
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
                var deviceInformation = _aquariumService.GetDeviceInformation(deviceId);
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
                var deviceInformation = _aquariumService.GetDeviceInformation(deviceId);
                return new OkObjectResult(deviceInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Information: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }


        //Create/remove device sensors
        [HttpGet]
        [Route("/v1/Device/{deviceId}/Sensors")]
        public IActionResult GetDeviceSensors(int deviceId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Sensors called");
                var deviceSensors = _aquariumService.GetDeviceSensors(deviceId);
                return new OkObjectResult(deviceSensors);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Sensors: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Device/{deviceId}/Sensor/Create")]
        public IActionResult CreateDeviceSensor(int deviceId, [FromBody] DeviceSensor sensor)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Sensor/Create called");
                var deviceSensor = _aquariumService.CreateDeviceSensor(deviceId,sensor);
                return new OkObjectResult(deviceSensor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Sensor/Create: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Device/{deviceId}/Sensor/Remove")]
        public IActionResult RemoveDeviceSensor(int deviceId,[FromBody] DeviceSensor sensor)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Sensor/Remove called");
                _aquariumService.DeleteDeviceSensor(deviceId,sensor.Id);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Sensor/Remove: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPut]
        [Route("/v1/Device/{deviceId}/Sensor/Update")]
        public IActionResult UpdateDeviceSensor(int deviceId, [FromBody] DeviceSensor sensor)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Sensor/Update called");
                sensor = _aquariumService.UpdateDeviceSensor(sensor);
                return new OkObjectResult(sensor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Sensor/Update: { ex.Message } Details: { ex.ToString() }");
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
                ScheduleState scheduleState = _aquariumService.GetDeviceScheduleStatus(deviceId);
                return new OkObjectResult(scheduleState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Schedule/Status: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("/v1/Device/{deviceId}/Schedule/PerformTask")]
        public IActionResult PerformScheduleTask(int deviceId,[FromBody] DeviceScheduleTask deviceScheduleTask)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Schedule/PerformTask called");
                _aquariumService.PerformScheduleTask(deviceId,deviceScheduleTask);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Schedule/Status: { ex.Message } Details: { ex.ToString() }");
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