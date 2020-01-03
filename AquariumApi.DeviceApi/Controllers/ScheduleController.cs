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
            IHostedService scheduleManagerService)
        {
            _config = config;
            _logger = logger;
            _scheduleService = scheduleManagerService as ScheduleService;

        }
        // GET schedule/ - Retrieve all schedules on this device
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Schedule called");
                var deviceSchedules = _scheduleService.GetAllSchedules();
                return new OkObjectResult(deviceSchedules);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Schedule endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
            
        }
        [HttpGet("Status")]
        public ActionResult<IEnumerable<string>> GetScheduleStatus()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Status called");
                var scheduleStatus = _scheduleService.GetStatus();
                return new OkObjectResult(scheduleStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Schedule endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
        [HttpPost]
        public IActionResult ApplyScheduleAssignment([FromBody] List<DeviceSchedule> deviceSchedules)
        {
            try
            {
                deviceSchedules.ForEach(s =>
                {
                    s.Host = _config["AquariumServiceUrl"];
                });
                _logger.LogInformation("POST /v1/Schedule called");
                _scheduleService.SaveSchedulesToCache(deviceSchedules);
                _scheduleService.StopAsync(_scheduleService.token).Wait();
                _scheduleService.StartAsync(new System.Threading.CancellationToken()).Wait();
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Schedule endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
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
                return NotFound();
            }
        }
    }
}
