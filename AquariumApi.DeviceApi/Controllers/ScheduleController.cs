using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private IConfiguration _config;
        private ILogger<ScheduleController> _logger;
        private ScheduleService _scheduleService;

        public ScheduleController(IConfiguration config,
            ILogger<ScheduleController> logger,
            ScheduleService scheduleManagerService)
        {
            _config = config;
            _logger = logger;
            _scheduleService = scheduleManagerService;

        }
        // GET schedule/ - Get schedule status
        [HttpGet]
        [Route(DeviceOutboundEndpoints.SCHEDULE_STATUS)]
        public ActionResult<IEnumerable<string>> GetScheduleStatus()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SCHEDULE_STATUS} called");
                var scheduleStatus = _scheduleService.GetStatus();
                return new OkObjectResult(scheduleStatus);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SCHEDULE_STATUS} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.SCHEDULE_STATUS} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        [HttpGet(DeviceOutboundEndpoints.SCHEDULE_START)]
        public IActionResult Start()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Schedule/Start called");
                _scheduleService.StopAsync(_scheduleService.token).Wait();
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
                _scheduleService.StopAsync(_scheduleService.token).Wait();
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
                _scheduleService.PerformTask(deviceScheduleTask);
                return new OkResult();
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
        [HttpGet(DeviceOutboundEndpoints.SCHEDULE_REMAINING_TASKS)]
        public IActionResult GetRemainingTasks()
        {
            try
            {
                var tasks = _scheduleService.GetAllScheduledTasks();
                return new OkObjectResult(tasks);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.SCHEDULE_REMAINING_TASKS} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.SCHEDULE_REMAINING_TASKS} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }

    }
}
