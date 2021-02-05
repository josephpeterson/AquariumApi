using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    [Authorize]
    public class AquariumController : Controller
    {
        private readonly IAccountService _accountService;
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<SnapshotController> _logger;
        public AquariumController(IAccountService accountService, IAquariumService aquariumService, ILogger<SnapshotController> logger)
        {
            _accountService = accountService;
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<Aquarium>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Aquarium/All")]
        public IActionResult All()
        {
            try
            {
                _logger.LogInformation("GET /v1/Aquarium/All called");
                var id = _accountService.GetCurrentUserId();
                var aquariums = _aquariumService.GetAquariumsByAccountId(id);
                return new OkObjectResult(aquariums);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquarium/All endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("/v1/Aquarium/{id}")]
        [ProducesResponseType(typeof(Aquarium), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetAquariumById(int id)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Aquarium/{id} called");
                var aquarium = _aquariumService.GetAquariumById(id);
                if (aquarium == null)
                    return NotFound();
                return new OkObjectResult(aquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquarium/{id} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

        [HttpPost]
        [Route("/v1/Aquarium/Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddAquarium([FromBody]Aquarium aquarium)
        {
            try
            {
                _logger.LogInformation("POST /v1/Aquarium/Add called");

                if (aquarium == null)
                    throw new Exception("No aquarium information sent");

                //Static properties
                int id = _accountService.GetCurrentUserId();
                aquarium.OwnerId = id;
                var newAquarium = _aquariumService.AddAquarium(aquarium);
                return CreatedAtAction(nameof(GetAquariumById), new { id = newAquarium.Id }, newAquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/Add caught exception: { ex.Message } Details: { ex.ToString() }");
                return new BadRequestObjectResult(ex.Message);
            }
        }
        [HttpPost]
        [Route("/v1/Aquarium/Update")]
        public IActionResult UpdateAquarium([FromBody] Aquarium updatedAquarium)
        {
            try
            {
                _logger.LogInformation("POST /v1/Aquarium/Update called");
                var id = _accountService.GetCurrentUserId();
                var aq = _aquariumService.GetAquariumById(updatedAquarium.Id);
                if (aq.OwnerId != id) return new UnauthorizedResult();
                var aquarium = _aquariumService.UpdateAquarium(updatedAquarium);
                return new OkObjectResult(aquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/Update endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Aquarium/Delete")]
        public IActionResult DeleteAquarium([FromBody] int removeAquariumId)
        {
            try
            {
                _logger.LogInformation("POST /v1/Aquarium/Delete called");
                var id = _accountService.GetCurrentUserId();
                var aq = _aquariumService.GetAquariumById(removeAquariumId);
                if (aq.OwnerId != id) return new UnauthorizedResult();
                _aquariumService.DeleteAquarium(removeAquariumId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Aquarium/TemperatureHistogram")]
        public IActionResult GetTemperatureHistogram(int aquariumId)
        {
            var id = _accountService.GetCurrentUserId();
            var aq = _aquariumService.GetAquariumById(aquariumId);
            if (aq.OwnerId != id) return new UnauthorizedResult();

            var temps = _aquariumService.GetAquariumTemperatureHistogram(aquariumId);

            return new OkObjectResult(temps);
        }
        [HttpGet]
        [Route("/v1/Aquarium/TemperatureHistogram/All")]
        public IActionResult GetTemperatureHistogramAll()
        {
            var id = _accountService.GetCurrentUserId();

            //var temps = new List<List<AquariumSnapshot>>();
            var aquariums = _aquariumService.GetAquariumsByAccountId(id).Select(aquarium =>
            {
                var snapshots = _aquariumService.GetAquariumTemperatureHistogram(aquarium.Id);
                aquarium.Snapshots = snapshots;
                return aquarium;
            }).ToList();
            return new OkObjectResult(aquariums);
        }

        [HttpPost]
        [Route("/v1/Aquarium/{aquariumId}/Snapshots")]
        public IActionResult GetAquariumSnapshots(int aquariumId, [FromBody] SnapshotSelection selection)
        {
            var id = _accountService.GetCurrentUserId();

            var aq = _aquariumService.GetAquariumById(aquariumId);
            if (aq.OwnerId != id) return new UnauthorizedResult();


            //var temps = new List<List<AquariumSnapshot>>();
            var snapshots = _aquariumService.GetAquariumSnapshots(aquariumId,selection.offset, selection.max);
            return new OkObjectResult(snapshots);
        }
        [HttpDelete]
        [Route("/v1/Aquarium/{aquariumId}/Snapshots")]
        public IActionResult DeleteAllAquariumSnapshsots(int aquariumId)
        {
            try
            {
                _logger.LogInformation($"DELETE /v1/Aquarium/{aquariumId}/Snapshots called");
                var id = _accountService.GetCurrentUserId();
                var aq = _aquariumService.GetAquariumById(aquariumId);
                if (aq.OwnerId != id) return new UnauthorizedResult();
                _aquariumService.DeleteAllSnapshots(aquariumId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE /v1/Aquarium/{aquariumId}/Snapshots endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
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
        public IActionResult PerformWaterChange(int aquariumId,[FromBody] WaterChange waterChange)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Aquarium/{aquariumId}/Water/Change called");
                var aq = _aquariumService.GetAquariumById(aquariumId);
                var id = _accountService.GetCurrentUserId();
                if(!_accountService.CanModify(id,aq))
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
        public IActionResult AlterWaterChange(int aquariumId,[FromBody] WaterChange waterChange)
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
        public IActionResult PerformWaterDosing(int aquariumId,[FromBody] WaterDosing waterDosing)
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
        public IActionResult AlterWaterDosing(int aquariumId,[FromBody] WaterDosing waterDosing)
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
    public class SnapshotSelection
    {
        public int offset = 0;
        public int max = 0;
    }
}