using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class SnapshotController : Controller
    {
        private readonly IConfiguration _config;
        public readonly IAquariumService _aquariumService;
        private readonly IActivityService _activityService;
        public readonly ILogger<SnapshotController> _logger;
        private readonly IPhotoManager _photoManager;

        public SnapshotController(IConfiguration config, IActivityService activityService, IAquariumService aquariumService, IDeviceService deviceService, ILogger<SnapshotController> logger,IPhotoManager photoManager)
        {
            _config = config;
            _aquariumService = aquariumService;
            _activityService = activityService;
            _logger = logger;
            _photoManager = photoManager;
        }
        [HttpGet]
        [Route("/v1/Snapshot/{id}/All")]
        [ProducesResponseType(typeof(List<AquariumSnapshot>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        //todo deprecate
        public IActionResult GetSnapshots(int id, [FromQuery] int count, [FromQuery] int offset = 0)
        {
            try
            {
                var data = _aquariumService.GetSnapshots(id);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Snapshot/{id}/All endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Snapshot/{aquariumId}/{snapshotId}")]
        [ProducesResponseType(typeof(List<AquariumSnapshot>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetSnapshotById(int aquariumId, int snapshotId, [FromQuery] int count, [FromQuery] int offset = 0)
        {
            try
            {
                var data = _aquariumService.GetSnapshots(aquariumId).Where(s => s.Id == snapshotId).First();
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Snapshot/{aquariumId}/GetSnapshotById endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
        [HttpPost]
        [Route("/v1/Snapshot/Delete")]
        public IActionResult DeleteSnapshot([FromBody] int removeSnapshotId)
        {
            try
            {
                _logger.LogInformation("POST /v1/Aquariums/Delete called");
                _aquariumService.DeleteSnapshot(removeSnapshotId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquariums endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Snapshot/{aquariumId}/Take/{takePhoto}")]
        [ProducesResponseType(typeof(List<AquariumSnapshot>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult TakeSnapshot(int aquariumId,bool takePhoto)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Snapshot/{aquariumId}/Take/{takePhoto} called");
                //Take snapshot
                var snapshot = _aquariumService.TakeSnapshot(aquariumId, takePhoto);
                return new OkObjectResult(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Snapshot/{aquariumId}/Take/{takePhoto} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Snapshot/{aquariumId}/Latest")]
        public IActionResult GetLatestSnapshot(int aquariumId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Snapshot/{aquariumId}/Latest called");
                //Take snapshot
                var snapshot = _aquariumService.GetSnapshots(aquariumId).Last();
                return new OkObjectResult(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Snapshot/{aquariumId}/Latest endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost, DisableRequestSizeLimit]
        [Route("/v1/Snapshot/{aquariumId}/Create")]
        public IActionResult UploadSnapshot(int aquariumId, IFormFile snapshotImage,AquariumSnapshot snapshot)
        {
            try
            {

                _logger.LogInformation($"POST /v1/Snapshot/{aquariumId}/Create called");
                snapshot.ManualEntry = true;
                AquariumSnapshot s = _aquariumService.AddSnapshot(aquariumId, snapshot, snapshotImage);

                //Activity Writer
                var uid = _aquariumService.GetAquariumById(aquariumId).OwnerId;
                _activityService.RegisterActivity(new CreateAquariumTestResultsActivity()
                {
                    AccountId = uid,
                    SnapshotId = snapshot.Id
                });

                return new OkObjectResult(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/v1/Device/Snapshot: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
    }
}