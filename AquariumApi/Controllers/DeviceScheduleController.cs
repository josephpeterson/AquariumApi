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

namespace AquariumApi.Controllers
{
    [Authorize]

    public class DeviceScheduleController : Controller
    {
        public readonly IAquariumService _aquariumService;
        private readonly IAccountService _accountService;
        public readonly IDeviceService _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceScheduleController(IAquariumService aquariumService, IAccountService accountService, IDeviceService deviceService,ILogger<DeviceController> logger)
        {
            _aquariumService = aquariumService;
            _accountService = accountService;
            _deviceService = deviceService;
            _logger = logger;
        }
        [HttpGet]
        [Route("/v1/Schedule")]
        public IActionResult GetDeviceSchedulesByAccountId()
        {
            try
            {
                _logger.LogInformation("GET /v1/Schedule called");
                var id = _accountService.GetCurrentUserId();
                var deviceSchedules = _aquariumService.GetDeviceSchedulesByAccountId(id);
                return new OkObjectResult(deviceSchedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Schedule called/ endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Schedule/Tasks")]
        public IActionResult GetScheduleTypes()
        {
            try
            {
                _logger.LogInformation("GET /v1/Schedule/Tasks called");
                var enumVals = new List<object>();

                foreach (var item in Enum.GetValues(typeof(ScheduleTaskTypes)))
                {

                    enumVals.Add(new
                    {
                        id = (int)item,
                        name = item.ToString()
                    });
                }

                return new OkObjectResult(enumVals);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Schedule called/ endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpDelete]
        [Route("/v1/Schedule/{scheduleId}/Delete")]
        public IActionResult DeviceDeviceSchedule(int scheduleId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Schedule/{scheduleId}/Delete called");
                _aquariumService.DeleteDeviceSchedule(scheduleId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Schedule/{scheduleId}/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Schedule/Update")]
        public IActionResult UpdateDeviceSchedule([FromBody] DeviceSchedule deviceSchedule)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Device/Update called");
                var updatedDeviceSchedule = _aquariumService.UpdateDeviceSchedule(deviceSchedule);
                return new OkObjectResult(updatedDeviceSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Schedule/Update endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Schedule/Add")]
        public IActionResult AddDeviceSchedule([FromBody] DeviceSchedule deviceSchedule)
        {
            try
            {
                deviceSchedule.AuthorId = _accountService.GetCurrentUserId();

                _logger.LogInformation($"POST /v1/Schedule/Add called");
                var newDeviceSchedule = _aquariumService.AddDeviceSchedule(deviceSchedule);
                return CreatedAtAction(nameof(UpdateDeviceSchedule), new { id = newDeviceSchedule.Id }, newDeviceSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Device/Add endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }}
}