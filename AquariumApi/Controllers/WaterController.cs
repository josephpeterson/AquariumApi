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
        private readonly IAccountService _accountService;
        public readonly ILogger<SnapshotController> _logger;
        private readonly IPhotoManager _photoManager;

        public WaterController(IConfiguration config, IAccountService accountService, IActivityService activityService, IAquariumService aquariumService, IDeviceClient deviceService, ILogger<SnapshotController> logger, IPhotoManager photoManager)
        {
            _config = config;
            _aquariumService = aquariumService;
            _activityService = activityService;
            _accountService = accountService;
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
                _logger.LogInformation($"POST /v1/Water/{aquariumId}/Parameters called");

                var data = _aquariumService.GetWaterParametersByAquarium(aquariumId, pagination);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Water/{aquariumId}/Parameters endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
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
        //Water change stuff
        [HttpGet]
        [Route("/v1/Aquarium/{aquariumId}/Water/Change")]
        public IActionResult GetWaterChanges(int aquariumId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Aquarium/{aquariumId}/Water called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var waterChanges = _aquariumService.GetWaterChangesByAquarium(aquariumId);
                return new OkObjectResult(waterChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquarium/{aquariumId}/Water endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Aquarium/{aquariumId}/Water/Change")]
        public IActionResult PerformWaterChange(int aquariumId, [FromBody] WaterChange waterChange)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Aquarium/{aquariumId}/Water/Change called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                waterChange.AquariumId = aquariumId;
                waterChange = _aquariumService.AddWaterChange(waterChange);
                return new OkObjectResult(waterChange);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/{aquariumId}/Water/Change endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPut]
        [Route("/v1/Aquarium/{aquariumId}/Water/Change")]
        public IActionResult AlterWaterChange(int aquariumId, [FromBody] WaterChange waterChange)
        {
            try
            {
                _logger.LogInformation("POST /v1/Aquarium/{aquariumId}/Water/Change called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                waterChange.AquariumId = aquariumId;
                waterChange = _aquariumService.UpdateWaterChange(waterChange);
                return new OkObjectResult(waterChange);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/{aquariumId}/Water/Change endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Aquarium/{aquariumId}/Water/Change/Delete")]
        public IActionResult DeleteWaterChanges(int aquariumId, [FromBody] List<int> waterChangeIds)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Aquarium/{aquariumId}/Water/Change/Delete called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var validWaterChanges = _aquariumService.GetWaterChangesByAquarium(aquariumId);
                var ids = validWaterChanges.Where(w => waterChangeIds.Contains(w.Id)).Select(w => w.Id).ToList();
                _aquariumService.DeleteWaterChanges(ids);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/{aquariumId}/Water/Change/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Aquarium/{aquariumId}/Water/Dose")]
        public IActionResult GetWaterDosing(int aquariumId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Aquarium/{aquariumId}/Water called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var waterChanges = _aquariumService.GetWaterDosingsByAquarium(aquariumId);
                return new OkObjectResult(waterChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquarium/{aquariumId}/Water endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Aquarium/{aquariumId}/Water/Dose")]
        public IActionResult PerformWaterDosing(int aquariumId, [FromBody] WaterDosing waterDosing)
        {
            try
            {
                _logger.LogInformation("POST /v1/Aquarium/{aquariumId}/Water/Dose called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                waterDosing.AquariumId = aquariumId;
                waterDosing = _aquariumService.AddWaterDosing(waterDosing);
                return new OkObjectResult(waterDosing);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/{aquariumId}/Water/Dose endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPut]
        [Route("/v1/Aquarium/{aquariumId}/Water/Dose")]
        public IActionResult AlterWaterDosing(int aquariumId, [FromBody] WaterDosing waterDosing)
        {
            try
            {
                _logger.LogInformation("POST /v1/Aquarium/{aquariumId}/Water/Dose called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                waterDosing.AquariumId = aquariumId;
                waterDosing = _aquariumService.UpdateWaterDosing(waterDosing);
                return new OkObjectResult(waterDosing);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/{aquariumId}/Water/Dose endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Aquarium/{aquariumId}/Water/Dose/Delete")]
        public IActionResult DeleteWaterDosings(int aquariumId, [FromBody] List<int> waterDosingIds)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Aquarium/{aquariumId}/Water/Dose/Delete called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var validWaterDosings = _aquariumService.GetWaterDosingsByAquarium(aquariumId);
                var ids = validWaterDosings.Where(w => waterDosingIds.Contains(w.Id)).Select(w => w.Id).ToList();
                _aquariumService.DeleteWaterDosings(ids);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/{aquariumId}/Water/Dose/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

    }
}