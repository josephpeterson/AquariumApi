using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        private IHardwareService _hardwareService;
        private ILogger<HomeController> _logger;

        public HomeController(IHardwareService hardwareService, ILogger<HomeController> logger)
        {
            _hardwareService = hardwareService;
            _logger = logger;
        }
        [HttpGet]
        [Route("/v1/Scan")]
        public IActionResult ScanHardware()
        {
            return new OkObjectResult(_hardwareService.ScanHardware());
        }
        [HttpGet]
        [Route("/v1/Ping")]
        public IActionResult Ping()
        {
            return new OkResult();
        }
    }
}
