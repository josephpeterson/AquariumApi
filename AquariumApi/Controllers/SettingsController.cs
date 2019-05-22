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
    public class SettingsController : Controller
    {
        private readonly IPhotoManager _photoManager;
        private readonly IConfiguration _config;
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<SnapshotController> _logger;
        public SettingsController(IConfiguration config, IPhotoManager photoManager, IAquariumService aquariumService, ILogger<SnapshotController> logger)
        {
            _photoManager = photoManager;
            _config = config;
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet]
        [Route("/v1/Settings/{id}")]
        [ProducesResponseType(typeof(List<AquariumSnapshot>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetSettings(int id)
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
    }
}