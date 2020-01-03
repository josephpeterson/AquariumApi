using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private IConfiguration _config;
        private ILogger<SettingsController> _logger;

        public SettingsController(IConfiguration config, ILogger<SettingsController> logger)
        {
            _config = config;
            _logger = logger;
        }
        [HttpGet]
        [Route("/v1/Log")]
        public IActionResult GetDeviceLog()
        {
            string txt;
            try
            {
                txt = System.IO.File.ReadAllText("AquariumDeviceApi.log");
            }
            catch (FileNotFoundException)
            {
                txt = "No error log found";
            }
            return new OkObjectResult(txt);
        }
        [HttpPost]
        [Route("/v1/Log/Clear")]
        public IActionResult ClearDeviceLog()
        {
            try
            {
                 System.IO.File.Delete("AquariumDeviceApi.log");
            }
            catch (Exception e)
            {
                return new NotFoundResult();
            }
            return new OkResult();
        }
    }
}
