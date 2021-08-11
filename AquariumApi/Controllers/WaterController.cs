using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    [Authorize]
    public class WaterController : Controller
    {
        private readonly IConfiguration _config;
        public readonly IAquariumService _aquariumService;
        private readonly IActivityService _activityService;
        public readonly ILogger<SnapshotController> _logger;
        private readonly IPhotoManager _photoManager;

        public WaterController(IConfiguration config, IActivityService activityService, IAquariumService aquariumService, IDeviceClient deviceService, ILogger<SnapshotController> logger, IPhotoManager photoManager)
        {
            _config = config;
            _aquariumService = aquariumService;
            _activityService = activityService;
            _logger = logger;
            _photoManager = photoManager;
        }
        [HttpPost]
        [Route("/v1/Water/{aquariumId}/Parameters")]
        [ProducesResponseType(typeof(List<AquariumSnapshot>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetWaterParameters(int aquariumId, [FromBody] PaginationSliver pagination = null)
        {
            try
            {
                var data = _aquariumService.GetWaterParametersByAquarium(aquariumId, pagination);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Snapshot/{aquariumId}/All endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Water/{aquariumId}/Parameters/Add")]
        [ProducesResponseType(typeof(List<AquariumSnapshot>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult AddWaterParameter(int aquariumId, [FromBody] AquariumSnapshot parameters)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Water/{aquariumId}/Parameters/Add called");
                AquariumSnapshot s = _aquariumService.AddWaterParametersByAquarium(aquariumId, parameters);

                //Activity Writer
                var uid = _aquariumService.GetAquariumById(aquariumId).OwnerId;
                _activityService.RegisterActivity(new CreateAquariumTestResultsActivity()
                {
                    AccountId = uid,
                    SnapshotId = s.Id
                });
                return new OkObjectResult(s);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Water/{aquariumId}/Parameters/Add: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
    }
}