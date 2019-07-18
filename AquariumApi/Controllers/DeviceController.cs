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
        [Route("/v1/Device/{deviceId}/Update")]
        public IActionResult UpdateAquariumDevice(int deviceId,[FromBody] AquariumDevice device)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/{deviceId}/Update called");
                device.Id = deviceId;
                var updatedDevice = _aquariumService.UpdateAquariumDevice(device);
                return new OkObjectResult(updatedDevice);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/{deviceId}/Update endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
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


        //Ping recieved
        [HttpPost]
        [Route("/v1/Device/Ping")]
        public IActionResult GetPingFromDevice([FromBody] AquariumDevice deviceRequest)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Device/Ping called");

                var host = Request.Host.Host;

                _logger.LogInformation($"\n\nHost: {host} \n {deviceRequest.PrivateKey}");

                var device = _aquariumService.GetAquariumDeviceByIpAndKey(host, deviceRequest.PrivateKey);
                _aquariumService.ApplyAquariumDeviceHardware(device.Id.Value, deviceRequest);
                return new OkObjectResult(device);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Device/Ping endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}