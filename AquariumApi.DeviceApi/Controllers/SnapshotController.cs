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

        public SnapshotController(ILogger<SnapshotController> logger,IPhotoManager photoManager)
        {
            _photoManager = photoManager;
            _logger = logger;
        }
        [HttpGet]
        [Route("/v1/Snapshot/Take")]
        public ActionResult<string> TakeSnapshot()
        {
            try
            {
                //_photoManager.TakePhoto(configuration);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }


            var snapshot = new AquariumSnapshot()
            {
                Date = DateTime.Now,
                Temperature = 0,
                Nitrate = 0.0M,
                Nitrite = 0.0M,
                Ph = 5.5M,
                //PhotoPath = path
            };
            return new OkObjectResult(snapshot);
        }

        [HttpPost]
        [Route("/v1/Snapshot/TakePhoto")]
        [ProducesResponseType(typeof(FileStream), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult TakePhoto([FromBody] CameraConfiguration configuration)
        {
            try
            {
                _logger.LogInformation($"POST /v1/TakePhoto called");
                configuration.Output = "temp.jpg";
                _photoManager.TakePhoto(configuration);
                _logger.LogInformation("Photo successfully taken");
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
