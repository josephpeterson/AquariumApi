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
    public class SpeciesController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<SpeciesController> _logger;
        public SpeciesController(IAquariumService aquariumService, ILogger<SpeciesController> logger)
        {
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<Aquarium>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Species")]
        public IActionResult GetAllSpecies()
        {
            try
            {
                _logger.LogInformation("GET /v1/Species called");
                List<Species> species = _aquariumService.GetAllSpecies();
                return new OkObjectResult(species);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Species endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Species/Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddSpecies([FromBody]Species species)
        {
            try
            {
                _logger.LogInformation("POST /v1/Species/Add called");
                var newSpecies = _aquariumService.AddSpecies(species);
                return CreatedAtAction(nameof(GetAllSpecies),new { id = newSpecies.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Species/Add caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Species/Update")]
        public IActionResult UpdateSpecies([FromBody] Species species)
        {
            try
            {
                _logger.LogInformation("POST /v1/Species/Update called");
                var updatedSpecies = _aquariumService.UpdateSpecies(species);
                return new OkObjectResult(updatedSpecies);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Species/Update endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Species/Delete")]
        public IActionResult DeleteSpecies([FromBody] int speciesId)
        {
            try
            {
                _logger.LogInformation("POST /v1/Species/Delete called");
                _aquariumService.DeleteSpecies(speciesId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Species/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}