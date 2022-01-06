using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.DeviceApi.Controllers
{
    public class SnapshotController : Controller
    {
        private IHardwareService _hardwareService;
        private ILogger<SnapshotController> _logger;

        public SnapshotController(ILogger<SnapshotController> logger,IHardwareService hardwareService)
        {
            _hardwareService = hardwareService;
            _logger = logger;
        }
        [HttpPost]
        [Route(DeviceOutboundEndpoints.HARDWARE_TAKE_PHOTO)]
        public IActionResult TakePhoto([FromBody] CameraConfiguration cameraConfiguration)
        {
            try
            {
                _logger.LogWarning($"POST {DeviceOutboundEndpoints.HARDWARE_TAKE_PHOTO} called");
                cameraConfiguration.Output = "temp.jpg";
                var photo = _hardwareService.TakePhoto(cameraConfiguration);
                _logger.LogWarning("Photo successfully taken");
                return File(photo,"image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {DeviceOutboundEndpoints.HARDWARE_TAKE_PHOTO} endpoint caught exception: {ex.Message} Details: {ex.ToString()}");
                _logger.LogError(ex.StackTrace);
                return NotFound();
            }
        }
    }
}
