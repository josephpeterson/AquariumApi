using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
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
        /* Load Water Parameter Store */

        #region Water Changes
        [HttpPost]
        [Route(AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_WATERCHANGES)]
        public IActionResult RetrieveWaterChanges(int aquariumId, [FromBody] PaginationSliver pagination)
        {
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_WATERCHANGES.AggregateParams($"{aquariumId}")} called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var waterChanges = _aquariumService.GetWaterChangesByAquarium(aquariumId, pagination);
                return new OkObjectResult(waterChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_WATERCHANGES.AggregateParams($"{aquariumId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_AUTOWATERCHANGES)]
        public IActionResult RetrieveAutoWaterChanges(int aquariumId, [FromBody] PaginationSliver pagination)
        {
            _logger.LogInformation($"POST {AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_AUTOWATERCHANGES.AggregateParams($"{aquariumId}")} called");
            pagination.CompareFunc = (o => ((WaterChange)o).ScheduleJobId.HasValue);
            return RetrieveWaterChanges(aquariumId, pagination);
        }

        [HttpPut]
        [Route(AquariumApiEndpoints.AQUARIUM_WATER_UPSERT_WATERCHANGE)]
        public IActionResult UpsertWaterChange(int aquariumId, [FromBody] WaterChange waterChange)
        {
            try
            {
                _logger.LogInformation($"PUT {AquariumApiEndpoints.AQUARIUM_WATER_UPSERT_WATERCHANGE.AggregateParams($"{aquariumId}")} called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                waterChange.AquariumId = aquariumId;
                waterChange = _aquariumService.UpsertWaterChange(waterChange);
                return new OkObjectResult(waterChange);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PUT {AquariumApiEndpoints.AQUARIUM_WATER_UPSERT_WATERCHANGE.AggregateParams($"{aquariumId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpDelete]
        [Route(AquariumApiEndpoints.AQUARIUM_WATER_DELETE_WATERCHANGES)]
        public IActionResult DeleteWaterChanges(int aquariumId, [FromBody] List<int> waterChangeIds)
        {
            try
            {
                _logger.LogInformation($"DELETE {AquariumApiEndpoints.AQUARIUM_WATER_DELETE_WATERCHANGES.AggregateParams($"{aquariumId}")} called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var validWaterChanges = _aquariumService.GetWaterChangesByAquarium(aquariumId);
                var ids = validWaterChanges.Where(w => waterChangeIds.Contains(w.Id.Value)).Select(w => w.Id.Value).ToList();
                _aquariumService.DeleteWaterChanges(ids);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE {AquariumApiEndpoints.AQUARIUM_WATER_DELETE_WATERCHANGES.AggregateParams($"{aquariumId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        #endregion

        [HttpPost]
        [Route(AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_WATERATOS)]
        public IActionResult RetrieveWaterATOs(int aquariumId, [FromBody] PaginationSliver pagination)
        {
            try
            {
                _logger.LogInformation($"POST {AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_WATERATOS.AggregateParams($"{aquariumId}")} called");
                var data = _aquariumService.GetWaterATOStatusesByAquarium(aquariumId, pagination);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_WATERATOS.AggregateParams($"{aquariumId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_AUTOWATERATOS)]
        public IActionResult RetrieveAutoWaterATOs(int aquariumId, [FromBody] PaginationSliver pagination)
        {
            _logger.LogInformation($"POST {AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_AUTOWATERCHANGES.AggregateParams($"{aquariumId}")} called");
            pagination.CompareFunc = (o => ((ATOStatus)o).ScheduleJobId.HasValue);
            return RetrieveWaterATOs(aquariumId, pagination);
        }

        [HttpPut]
        [Route(AquariumApiEndpoints.AQUARIUM_WATER_UPSERT_WATERATO)]
        public IActionResult UpsertWaterATO(int aquariumId, [FromBody] ATOStatus waterATO)
        {
            try
            {
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();
                waterATO.AquariumId = aquariumId;

                _logger.LogInformation($"POST {AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_WATERATOS.AggregateParams($"{aquariumId}")} called");
                var data = _aquariumService.UpsertWaterATO(waterATO);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST {AquariumApiEndpoints.AQUARIUM_WATER_RETRIEVE_WATERATOS.AggregateParams($"{aquariumId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpDelete]
        [Route(AquariumApiEndpoints.AQUARIUM_WATER_DELETE_WATERATOS)]
        public IActionResult DeleteWaterATOs(int aquariumId, [FromBody] List<int> waterATOIds)
        {
            try
            {
                _logger.LogInformation($"DELETE {AquariumApiEndpoints.AQUARIUM_WATER_DELETE_WATERATOS.AggregateParams($"{aquariumId}")} called");
                _aquariumService.DeleteWaterATOs(waterATOIds);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE {AquariumApiEndpoints.AQUARIUM_WATER_DELETE_WATERATOS.AggregateParams($"{aquariumId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        /* Get Just water parameters */
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
                    SnapshotId = s.Id.Value
                });
                return new OkObjectResult(s);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Water/{aquariumId}/Parameters/Add: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Water/{aquariumId}/Dose")]
        public IActionResult GetWaterDosing(int aquariumId,[FromBody] PaginationSliver pagination)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Water/{aquariumId}/Dose called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if (!_accountService.CanModify(id, aq))
                    return new UnauthorizedResult();

                var waterChanges = _aquariumService.GetWaterDosingsByAquarium(aquariumId,pagination);
                return new OkObjectResult(waterChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Water/{aquariumId}/Dose endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
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
                var ids = validWaterDosings.Where(w => waterDosingIds.Contains(w.Id.Value)).Select(w => w.Id.Value).ToList();
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