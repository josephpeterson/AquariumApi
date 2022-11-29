using System;
using System.Collections.Generic;
using AquariumApi.DeviceApi;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class DeviceScheduleController : Controller
    {
        public readonly ILogger<DeviceScheduleController> _logger;
        private readonly ScheduleService _scheduleService;

        public DeviceScheduleController(
            ILogger<DeviceScheduleController> logger,
            ScheduleService scheduleService
            )
        {
            _logger = logger;
            _scheduleService = scheduleService;
        }
        
        /// <summary>
        /// Update the current list of device schedules on the device.
        /// </summary>
        /// <param name="deviceSchedules">List of DeviceSchedule objects</param>
        /// <returns>DeviceSchedule list</returns>
        [HttpPost]
        [Route(DeviceOutboundEndpoints.SCHEDULE_UPDATE)]
        public IActionResult ApplySchedules([FromBody] List<DeviceSchedule> deviceSchedules)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SCHEDULE_UPDATE} called");
                deviceSchedules = _scheduleService.ApplyDeviceSchedules(deviceSchedules);
                return new OkObjectResult(deviceSchedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SCHEDULE_UPDATE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieve current list of set schedules.
        /// </summary>
        /// <returns></returns>
        [HttpGet(DeviceOutboundEndpoints.SCHEDULE_RETRIEVE)]
        public IActionResult GetAllSchedules()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SCHEDULE_RETRIEVE} called");
                var deviceSchedules = _scheduleService.GetAllSchedules();
                return new OkObjectResult(deviceSchedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.SCHEDULE_RETRIEVE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        /// <summary>
        /// Retrieve all registered ScheduleTaskTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet(DeviceOutboundEndpoints.SCHEDULE_RETRIEVE_TASK_TYPES)]
        public IActionResult GetScheduleTypes()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SCHEDULE_RETRIEVE_TASK_TYPES} called");
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
                _logger.LogError($"GET {DeviceOutboundEndpoints.SCHEDULE_RETRIEVE_TASK_TYPES} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        /// <summary>
        /// This endpoint returns scheduled jobs from the database (tasks that have been queued or sent for a run)
        /// </summary>
        /// <returns>
        /// List of ScheduledJobs that fit within the provided pagination sliver
        /// </returns>
        /// 
        /*
        [HttpPost]
        [Route(AquariumApiEndpoints.SCHEDULE_RETRIEVE_SCHEDULED_JOBS)]
        public IActionResult GetAllScheduledJobs(int deviceId,[FromBody] PaginationSliver pagination)
        {
            if (!ValidateRequest(deviceId))
                return Unauthorized();

            try
            {
                _logger.LogInformation($"GET {AquariumApiEndpoints.SCHEDULE_RETRIEVE_SCHEDULED_JOBS.AggregateParams($"{deviceId}")} called");
                List <ScheduledJob> scheduledJobs = _aquariumService.GetDeviceScheduledJobs(deviceId,pagination);
                return new OkObjectResult(scheduledJobs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {AquariumApiEndpoints.SCHEDULE_RETRIEVE_SCHEDULED_JOBS.AggregateParams($"{deviceId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        */
    }
}