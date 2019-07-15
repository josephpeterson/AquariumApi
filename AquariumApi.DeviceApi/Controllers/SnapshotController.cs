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
        private IPhotoManager _photoManager;
        private ILogger<SnapshotController> _logger;
        private ISerialService _serialService;
        private IScheduleService _scheduleService;

        public SnapshotController(ILogger<SnapshotController> logger,IPhotoManager photoManager,ISerialService serialService, IScheduleService scheduleService)
        {
            _photoManager = photoManager;
            _logger = logger;
            _serialService = serialService;
            _scheduleService = scheduleService;
        }
        [HttpGet]
        [Route("/v1/Snapshot/Take")]
        public ActionResult<string> TakeSnapshot()
        {
            try
            {
                _logger.LogWarning($"POST /v1/Take called");
                var snapshot = new AquariumSnapshot()
                {
                    Date = DateTime.Now,
                    Temperature = (_serialService.CanRetrieveTemperature() ? _serialService.GetTemperatureC() : 0),
                    Nitrate = (_serialService.CanRetrieveNitrate() ? _serialService.GetNitrate() : 0.00M),
                    Nitrite = (_serialService.CanRetrieveNitrite() ? _serialService.GetNitrite() : 0.00M),
                    Ph = (_serialService.CanRetrievePh() ? _serialService.GetPh() : 0.00M),
                };
                return new OkObjectResult(snapshot);
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
                _photoManager.TakePhoto(configuration);
                _logger.LogWarning("Photo successfully taken");
                return new FileStreamResult(new FileStream(configuration.Output, FileMode.Open, FileAccess.Read),
                    "application/octet-stream")
                {
                    FileDownloadName = configuration.Output
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/TakePhoto endpoint caught exception: {ex.Message} Details: {ex.ToString()}");
                return NotFound();
            }
        }
    }
}
