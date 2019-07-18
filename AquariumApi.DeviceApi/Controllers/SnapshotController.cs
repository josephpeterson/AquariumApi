using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    public class SnapshotController : Controller
    {
        private IDeviceService _deviceService;
        private ILogger<SnapshotController> _logger;
        private IScheduleService _scheduleService;

        public SnapshotController(ILogger<SnapshotController> logger,IDeviceService deviceService, IScheduleService scheduleService)
        {
            _deviceService = deviceService;
            _logger = logger;
            _scheduleService = scheduleService;
        }
        [HttpGet]
        [Route("/v1/Snapshot/Take")]
        public ActionResult<string> TakeSnapshot()
        {
            try
            {
                _logger.LogWarning($"POST /v1/Take called");
                return new OkObjectResult(_deviceService.TakeSnapshot());
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Take endpoint caught exception: {ex.Message} Details: {ex.ToString()}");
                return NotFound();
            }
        }

        [HttpPost]
        [Route("/v1/Snapshot/TakePhoto")]
        [ProducesResponseType(typeof(FileStream), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult TakePhoto([FromBody] CameraConfiguration configuration)
        {
            try
            {
                _logger.LogWarning($"POST /v1/TakePhoto called");
                configuration.Output = "temp.jpg";
                var photo = _deviceService.TakePhoto(configuration);
                _logger.LogWarning("Photo successfully taken");
                return photo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/TakePhoto endpoint caught exception: {ex.Message} Details: {ex.ToString()}");
                return NotFound();
            }
        }
    }
}
