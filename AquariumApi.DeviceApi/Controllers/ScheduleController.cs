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
        
    }

}
