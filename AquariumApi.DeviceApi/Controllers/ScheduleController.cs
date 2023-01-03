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

        [HttpGet(DeviceOutboundEndpoints.HARDWARE_RETRIEVE_CAMERA_DEVICES)]
        public IActionResult GetAllCameraDevices()
        {
            try
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.HARDWARE_RETRIEVE_CAMERA_DEVICES} called");
                var devices = GetAllCameraDevices();
                return new OkObjectResult(devices);
            }
            catch (DeviceException ex)
            {
                _logger.LogInformation($"GET {DeviceOutboundEndpoints.HARDWARE_RETRIEVE_CAMERA_DEVICES} endpoint caught exception: {ex.Message}");
                return BadRequest(new DeviceException(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {DeviceOutboundEndpoints.HARDWARE_RETRIEVE_CAMERA_DEVICES} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return BadRequest(new DeviceException("Unknown device error occurred")
                {
                    Source = ex
                });
            }
        }
        public static List<string> GetAllConnectedCameras()
        {
            throw new NotImplementedException();
        }
    }

}
