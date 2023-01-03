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

namespace AquariumApi.Controllers.DeviceInteraction
{
    [Authorize]
    [Route("DeviceInteraction")]
    public class DeviceInteractionScheduleController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAquariumService _aquariumService;
        private readonly IAquariumDeviceInteractionService _deviceService;
        public readonly ILogger<DeviceController> _logger;
        public DeviceInteractionScheduleController(IAccountService accountService
            ,IAquariumService aquariumService
            ,IAquariumDeviceInteractionService deviceService
            ,ILogger<DeviceController> logger)
        {
            _accountService = accountService;
            _aquariumService = aquariumService;
            _deviceService = deviceService;
            _logger = logger;
        }
        private bool ValidateRequest(int aquariumId)
        {
            var id = _accountService.GetCurrentUserId();
            var aquariums = _aquariumService.GetAquariumsByAccountId(id);
            return aquariums.Where(a => a.Id == aquariumId && a.Device != null).Any();
        }
        [HttpGet("{aquariumId}" + DeviceOutboundEndpoints.SCHEDULE_START)]
        public async Task<IActionResult> Start(int aquariumId)
        {
            try
            {
                _logger.LogInformation($"GET {aquariumId + DeviceOutboundEndpoints.SCHEDULE_START} called");
                ScheduleState scheduleStatus = await _deviceService.StartDeviceSchedule(aquariumId);
                return new OkObjectResult(scheduleStatus);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {aquariumId + DeviceOutboundEndpoints.SCHEDULE_START} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {aquariumId + DeviceOutboundEndpoints.SCHEDULE_START} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpGet("{aquariumId}" + DeviceOutboundEndpoints.SCHEDULE_STOP)]
        public async Task<IActionResult> Stop(int aquariumId)
        {
            try
            {
                _logger.LogInformation($"GET {aquariumId + DeviceOutboundEndpoints.SCHEDULE_STOP} called");
                ScheduleState scheduleStatus = await _deviceService.StopDeviceSchedule(aquariumId);
                return new OkObjectResult(scheduleStatus);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {aquariumId + DeviceOutboundEndpoints.SCHEDULE_STOP} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {aquariumId + DeviceOutboundEndpoints.SCHEDULE_STOP} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpPost("{aquariumId}" + DeviceOutboundEndpoints.SCHEDULE_TASK_PERFORM)]
        public async Task<IActionResult> PerformTask(int aquariumId, [FromBody] DeviceScheduleTask deviceScheduleTask)
        {
            try
            {
                var runningScheduledJob = await _deviceService.PerformDeviceTask(aquariumId,deviceScheduleTask);
                return new OkObjectResult(runningScheduledJob.ScheduledJob);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {aquariumId + DeviceOutboundEndpoints.SCHEDULE_TASK_PERFORM} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {aquariumId + DeviceOutboundEndpoints.SCHEDULE_TASK_PERFORM} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpPost("{aquariumId}" + DeviceOutboundEndpoints.SCHEDULE_SCHEDULEDJOB_STOP)]
        public async Task<IActionResult> StopScheduledJob(int aquariumId,[FromBody] ScheduledJob scheduledJob)
        {
            try
            {
                var stoppedJob = await _deviceService.StopScheduledJob(aquariumId,scheduledJob);
                return new OkObjectResult(stoppedJob);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"POST {aquariumId + DeviceOutboundEndpoints.SCHEDULE_SCHEDULEDJOB_STOP} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {aquariumId + DeviceOutboundEndpoints.SCHEDULE_SCHEDULEDJOB_STOP} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }

    }
}