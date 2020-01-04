﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        private IDeviceService _deviceService;
        private ScheduleService _scheduleService;
        private ILogger<HomeController> _logger;
        private IConfiguration _config;

        public HomeController(IDeviceService deviceService,
            ScheduleService scheduleService,
            ILogger<HomeController> logger, IConfiguration config)
        {
            _deviceService = deviceService;
            _scheduleService = scheduleService;
            _logger = logger;
            _config = config;
        }
        [HttpGet]
        [Route("/v1/Scan")]
        public IActionResult ScanHardware()
        {
            try
            {
                _logger.LogInformation("GET /v1/Scan called");
                _deviceService.CheckAvailableHardware();
                return new OkObjectResult(_deviceService.GetDevice());
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Scan endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Ping")]
        public IActionResult Ping([FromBody] AquariumDevice device)
        {
            try
            {
                _logger.LogInformation("POST /v1/Ping called");
                _deviceService.SetDevice(device);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Ping endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Ping")]
        public IActionResult CheckPing()
        {
            _logger.LogInformation("GET /v1/Ping called");
            return new OkResult();
        }
        [HttpGet]
        [Route("/v1/Information")]
        public IActionResult GetDeviceInformation()
        {
            try
            {
                _logger.LogInformation("POST /v1/Information called");
                var information = new DeviceInformation()
                {
                    Aquarium = _deviceService.GetDevice().Aquarium,
                    config = System.IO.File.ReadAllText("config.json"),
                    Schedules = _scheduleService.GetAllSchedules()
                };
                return new OkObjectResult(information);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Information endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
    }
}
