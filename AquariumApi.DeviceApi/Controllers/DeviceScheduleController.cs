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

        [HttpGet(DeviceOutboundEndpoints.SCHEDULE_STATUS)]
        public IActionResult GetDeviceScheduleStatus()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SCHEDULE_STATUS} called");
                var scheduleStatus = _scheduleService.GetScheduleStatus();
                return new OkObjectResult(scheduleStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.SCHEDULE_STATUS}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Update the current list of device schedules on the device.
        /// </summary>
        /// <param name="deviceSchedules">List of DeviceSchedule objects</param>
        /// <returns>DeviceSchedule list</returns>
        [HttpPost(DeviceOutboundEndpoints.SCHEDULE_UPSERT)]
        public IActionResult UpsertSchedule([FromBody] DeviceSchedule deviceSchedule)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SCHEDULE_UPSERT} called");
                var deviceSchedules = _scheduleService.UpsertDeviceSchedule(deviceSchedule);
                return new OkObjectResult(deviceSchedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SCHEDULE_UPSERT}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(ex.Message);
            }
        }
        [HttpPost(DeviceOutboundEndpoints.SCHEDULE_DELETE)]
        public IActionResult DeleteSchedule([FromBody] DeviceSchedule deviceSchedule)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SCHEDULE_DELETE} called");
                var deviceSchedules = _scheduleService.RemoveDeviceSchedule(deviceSchedule);
                return new OkObjectResult(deviceSchedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SCHEDULE_DELETE}: { ex.Message } Details: { ex.ToString() }");
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
        [HttpGet(DeviceOutboundEndpoints.TASK_RETRIEVE)]
        public IActionResult GetAllTasks()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.TASK_RETRIEVE} called");
                var deviceTasks = _scheduleService.GetAllTasks();
                return new OkObjectResult(deviceTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.TASK_RETRIEVE} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route(DeviceOutboundEndpoints.TASK_UPSERT)]
        public IActionResult UpsertTask([FromBody] DeviceScheduleTask deviceTask)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.TASK_UPSERT} called");
                var tasks = _scheduleService.UpsertDeviceTask(deviceTask);
                return new OkObjectResult(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.TASK_UPSERT}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(ex.Message);
            }
        }
        [HttpPost(DeviceOutboundEndpoints.TASK_DELETE)]
        public IActionResult DeleteTask([FromBody] DeviceScheduleTask deviceTask)
        {
            try
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.TASK_DELETE} called");
                var tasks = _scheduleService.RemoveDeviceTask(deviceTask);
                return new OkObjectResult(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.TASK_DELETE}: { ex.Message } Details: { ex.ToString() }");
                return BadRequest(ex.Message);
            }
        }
        [HttpGet(DeviceOutboundEndpoints.SCHEDULE_START)]
        public IActionResult Start()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Schedule/Start called");
                _scheduleService.StopAsync(_scheduleService._cancellationSource.Token).Wait();
                _scheduleService.StartAsync(new System.Threading.CancellationToken()).Wait();
                return new OkResult();
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SCHEDULE_START} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.SCHEDULE_START} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpGet(DeviceOutboundEndpoints.SCHEDULE_STOP)]
        public ActionResult<IEnumerable<string>> Stop()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SCHEDULE_STOP} called");
                _scheduleService.StopAsync(_scheduleService._cancellationSource.Token).Wait();
                return new OkResult();
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SCHEDULE_STOP} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.SCHEDULE_STOP} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpPost(DeviceOutboundEndpoints.SCHEDULE_TASK_PERFORM)]
        public IActionResult PerformTask([FromBody] DeviceScheduleTask deviceScheduleTask)
        {
            try
            {
                var runningScheduledJob = _scheduleService.GenericPerformTask(deviceScheduleTask);
                return new OkObjectResult(runningScheduledJob.ScheduledJob);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SCHEDULE_TASK_PERFORM} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SCHEDULE_TASK_PERFORM} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpPost(DeviceOutboundEndpoints.SCHEDULE_SCHEDULEDJOB_STOP)]
        public IActionResult StopScheduledJob([FromBody] ScheduledJob scheduledJob)
        {
            try
            {
                var stoppedJob = _scheduleService.StopScheduledJob(scheduledJob,JobEndReason.ForceStop);
                return new OkObjectResult(stoppedJob);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {DeviceOutboundEndpoints.SCHEDULE_SCHEDULEDJOB_STOP} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.SCHEDULE_SCHEDULEDJOB_STOP} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
    }
}