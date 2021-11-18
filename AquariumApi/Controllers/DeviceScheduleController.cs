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
        public readonly IDeviceClient _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceScheduleController(IAquariumService aquariumService, IAccountService accountService, IDeviceClient deviceService,ILogger<DeviceController> logger)
        {
            _aquariumService = aquariumService;
            _accountService = accountService;
            _deviceService = deviceService;
            _logger = logger;
        }
        /* Retrieve all user created schedules. */
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
        /// <summary>
        /// This endpoint returns scheduled jobs from the database (tasks that have been queued or sent for a run)
        /// </summary>
        /// <returns>
        /// List of ScheduledJobs that fit within the provided pagination sliver
        /// </returns>
        [HttpGet]
        [Route("/v1/Device/{deviceId}/Schedule/Jobs")]
        public IActionResult GetScheduledJobs(int deviceId, [FromBody] PaginationSliver pagination = null)
        {
            if (pagination == null)
                pagination = new PaginationSliver();
            try
            {
                _logger.LogInformation($"GET /v1/Device/{deviceId}/Schedule/Jobs called");
                List<ScheduledJob> scheduledJobs = _aquariumService.GetDeviceScheduledJobs(deviceId, pagination);
                return new OkObjectResult(scheduledJobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Device/{deviceId}/Schedule/Jobs endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
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