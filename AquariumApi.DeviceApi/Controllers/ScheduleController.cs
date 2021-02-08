using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [Route("v1/[controller]")]
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
        public ActionResult<IEnumerable<string>> GetScheduleStatus()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Schedule/Status called");
                var scheduleStatus = _scheduleService.GetStatus();
                return new OkObjectResult(scheduleStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Schedule/Status endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }

        }
        [HttpGet("Start")]
        public IActionResult Start()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Schedule/Start called");
                _scheduleService.StopAsync(_scheduleService.token).Wait();
                _scheduleService.StartAsync(new System.Threading.CancellationToken()).Wait();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Schedule/Start endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet("Stop")]
        public ActionResult<IEnumerable<string>> Stop()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Schedule/Stop called");
                _scheduleService.StopAsync(_scheduleService.token).Wait();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Schedule/Stop caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpPost("PerformTask")]
        public IActionResult PerformTask([FromBody] DeviceScheduleTask deviceScheduleTask)
        {
            try
            {
                _scheduleService.PerformTask(deviceScheduleTask);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/PerformTask endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet("Scheduled")]
        public IActionResult GetRemainingTasks()
        {
            try
            {
                var tasks = _scheduleService.GetAllScheduledTasks();
                return new OkObjectResult(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Remaining endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }

    }
}
