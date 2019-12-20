using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        private IDeviceService _deviceService;
        private ILogger<HomeController> _logger;

        public HomeController(IDeviceService deviceService, ILogger<HomeController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
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
    }
}
