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
        private IHardwareService _hardwareService;
        private ILogger<HomeController> _logger;
        private Aquarium _aquarium;

        public HomeController(IHardwareService hardwareService, ILogger<HomeController> logger,Aquarium aquarium)
        {
            _hardwareService = hardwareService;
            _logger = logger;
            _aquarium = aquarium;
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
        [HttpPost]
        [Route("/v1/Aquarium")]
        public IActionResult SetAquarium([FromBody] Aquarium aquarium)
        {
            Console.WriteLine("setingsg the aquarikum");
            _aquarium.Id = aquarium.Id;
            _aquarium.Name = aquarium.Name;
            _aquarium.Type = aquarium.Type;
            _aquarium.Gallons = aquarium.Gallons;
            _aquarium.StartDate = aquarium.StartDate;
            _logger.LogWarning("Aquarium set");
            return new OkResult();
        }
    }
}
