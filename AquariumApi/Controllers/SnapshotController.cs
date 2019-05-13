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
        private readonly IPhotoManager _photoManager;
        private readonly IConfiguration _config;
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<SnapshotController> _logger;
        public SnapshotController(IConfiguration config,IPhotoManager photoManager,IAquariumService aquariumService, ILogger<SnapshotController> logger)
        {
            _photoManager = photoManager;
            _config = config;
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet]
        [Route("/v1/Snapshot/{id}/All")]
        [ProducesResponseType(typeof(List<AquariumSnapshot>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetSnapshots(int id,[FromQuery] int count, [FromQuery] int offset = 0)
        {
            var data = _aquariumService.GetSnapshots(id);
            return new OkObjectResult(data);
        }
        [HttpGet]
        [Route("/v1/Snapshot/{aquariumId}/{snapshotId}")]
        [ProducesResponseType(typeof(List<AquariumSnapshot>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetSnapshotById(int aquariumId, int snapshotId, [FromQuery] int count, [FromQuery] int offset = 0)
        {
            var data = _aquariumService.GetSnapshots(aquariumId).Where(s => s.Id == snapshotId).First();
            return new OkObjectResult(data);
        }
        [HttpGet]
        [Route("/v1/Snapshot/{aquariumId}/Take")]
        [ProducesResponseType(typeof(List<AquariumSnapshot>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult TakeSnapshot(int aquariumId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Snapshot/{aquariumId}/Take called");
                //Take snapshot
                var snapshot = _aquariumService.TakeSnapshot(aquariumId);
                //Take photo
                var folder = _config["PhotoSubPath"] + $"{aquariumId}";
                var destination = $"{folder}/{snapshot.Id}.jpg";
                Directory.CreateDirectory(folder);
                _logger.LogInformation($"Taking photo snapshot {snapshot.Id}...");
                var photo = _photoManager.TakePhoto().Result;
                _logger.LogInformation($"Moving photo {photo}...");
                System.IO.File.Move(photo, destination);

                return new OkObjectResult(snapshot);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Snapshot/{aquariumId}/Take endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}