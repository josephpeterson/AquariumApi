using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class AquariumController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<SnapshotController> _logger;
        public AquariumController(IAquariumService aquariumService, ILogger<SnapshotController> logger)
        {
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
                _logger.LogInformation("GET /v1/Aquariums called");
                var aquariums = _aquariumService.GetAllAquariums();
                return new OkObjectResult(aquariums);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquariums endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
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
                var newAquarium = _aquariumService.AddAquarium(aquarium);
                return CreatedAtAction(nameof(GetAquariumById),new { id = newAquarium.Id }, newAquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Aquarium/Add caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Aquarium/Update")]
        public IActionResult UpdateAquarium([FromBody] Aquarium updatedAquarium)
        {
            try
            {
                _logger.LogInformation("POST /v1/Aquariums/Update called");
                var aquarium = _aquariumService.UpdateAquarium(updatedAquarium);
                return new OkObjectResult(aquarium);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquariums endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Aquarium/Delete")]
        public IActionResult DeleteAquarium([FromBody] int removeAquariumId)
        {
            try
            {
                _logger.LogInformation("POST /v1/Aquariums/Delete called");
                _aquariumService.DeleteAquarium(removeAquariumId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Aquariums endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}