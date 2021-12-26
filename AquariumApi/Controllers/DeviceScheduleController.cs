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

namespace AquariumApi.Controllers
{
    [Authorize]

    public class DeviceScheduleController : Controller
    {
        public readonly IAquariumService _aquariumService;
        private readonly IAccountService _accountService;
        public readonly IDeviceClient _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceScheduleController(IAquariumService aquariumService, IAccountService accountService, IDeviceClient deviceService, ILogger<DeviceController> logger)
        {
            _aquariumService = aquariumService;
            _accountService = accountService;
            _deviceService = deviceService;
            _logger = logger;
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.SCHEDULE_CREATE)]
        public IActionResult CreateDeviceSchedule(int deviceId, [FromBody] DeviceSchedule deviceSchedule)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.SCHEDULE_CREATE.AggregateParams($"{deviceId}")} called");
                deviceSchedule = _aquariumService.CreateDeviceSchedule(deviceId, deviceSchedule);
                return new OkObjectResult(deviceSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.SCHEDULE_CREATE.AggregateParams($"{deviceId}")}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(ex.Message);
            }
        }

        /* Retrieve all user created schedules. */
        [HttpGet]
        [Route(AquariumApiEndpoints.SCHEDULE_RETRIEVE)]
        public IActionResult GetDeviceSchedulesByAccountId()
        {
            try
            {
                _logger.LogInformation($"GET {AquariumApiEndpoints.SCHEDULE_RETRIEVE} called");
                var id = _accountService.GetCurrentUserId();
                var deviceSchedules = _aquariumService.GetDeviceSchedulesByAccountId(id);
                return new OkObjectResult(deviceSchedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {AquariumApiEndpoints.SCHEDULE_RETRIEVE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route(AquariumApiEndpoints.SCHEDULE_RETRIEVE_TASKTYPES)]
        public IActionResult GetScheduleTypes()
        {
            try
            {
                _logger.LogInformation($"GET {AquariumApiEndpoints.SCHEDULE_RETRIEVE_TASKTYPES} called");
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
                _logger.LogError($"GET {AquariumApiEndpoints.SCHEDULE_RETRIEVE_TASKTYPES} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
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
        [Route(AquariumApiEndpoints.SCHEDULE_RETRIEVE_SCHEDULED_JOBS_ON_DEVICE)]
        public IActionResult GetScheduledJobsOnDevice(int deviceId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"GET {AquariumApiEndpoints.SCHEDULE_RETRIEVE_SCHEDULED_JOBS_ON_DEVICE.AggregateParams($"{deviceId}")} called");
                List <ScheduledJob> scheduledJobs = _aquariumService.GetScheduledJobsOnDevice(deviceId);
                return new OkObjectResult(scheduledJobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {AquariumApiEndpoints.SCHEDULE_RETRIEVE_SCHEDULED_JOBS_ON_DEVICE.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route(AquariumApiEndpoints.SCHEDULE_DELETE)]
        public IActionResult DeviceDeviceSchedule(int deviceId,int scheduleId)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();
            try
            {
                _logger.LogInformation($"DELETE {AquariumApiEndpoints.SCHEDULE_DELETE.AggregateParams($"{deviceId}",$"{scheduleId}")} called");
                _aquariumService.DeleteDeviceSchedule(deviceId,scheduleId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE {AquariumApiEndpoints.SCHEDULE_DELETE.AggregateParams($"{deviceId}", $"{scheduleId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.SCHEDULE_SCHEDULED_JOB_STOP)]
        public IActionResult StopScheduledJob(int deviceId,[FromBody] ScheduledJob scheduledJob)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.SCHEDULE_SCHEDULED_JOB_STOP.AggregateParams($"{deviceId}")} called");
                var stoppedJob = _aquariumService.StopScheduledJob(deviceId, scheduledJob);
                return new OkObjectResult(stoppedJob);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.SCHEDULE_SCHEDULED_JOB_STOP.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
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