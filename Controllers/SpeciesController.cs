using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class SpeciesController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly IWebScraperService _webScraperService;
        public readonly ILogger<SpeciesController> _logger;
        public SpeciesController(IAquariumService aquariumService, IWebScraperService webScraperService,ILogger<SpeciesController> logger)
        {
            _aquariumService = aquariumService;
            _webScraperService = webScraperService;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<Species>), StatusCodes.Status200OK)]
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
        [HttpGet]
        [ProducesResponseType(typeof(Species), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Species/{speciesId}")]
        public IActionResult GetSpeciesById(int speciesId)
        {
            try
            {
                _logger.LogInformation("GET /v1/Species called");
                Species species = _aquariumService.GetSpeciesById(speciesId);
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

                return ScrapeSourcesForSpecies(newSpecies.Id);
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
        [HttpGet]
        [ProducesResponseType(typeof(Species), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Species/{speciesId}/Scrape")]
        public IActionResult ScrapeSourcesForSpecies(int speciesId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Species/{speciesId}/Scrape called");

                Species species = _aquariumService.GetSpeciesById(speciesId);
                _webScraperService.ApplyWebpageToSpecies(species.Website,species);
                _aquariumService.UpdateSpecies(species);
                return new OkObjectResult(species);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Species endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [ProducesResponseType(typeof(Species), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Species/All/Scrape")]
        public IActionResult ScrapeSourcesForAllSpecies()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Species/All/Scrape called");

                var speciesList = _aquariumService.GetAllSpecies();
                var list = new List<Species>();
                speciesList.ForEach(s =>
                {
                    Species species = _aquariumService.GetSpeciesById(s.Id);
                    _webScraperService.ApplyWebpageToSpecies(species.Website, species);
                    _aquariumService.UpdateSpecies(species);
                    list.Add(species);
                });
                return new OkObjectResult(list);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Species endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}