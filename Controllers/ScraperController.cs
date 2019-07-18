using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Core.Services;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class ScraperController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly IWebScraperService _webScraperService;
        public readonly ILogger<SpeciesController> _logger;
        public ScraperController(IAquariumService aquariumService, IWebScraperService webScraperService,ILogger<SpeciesController> logger)
        {
            _aquariumService = aquariumService;
            _webScraperService = webScraperService;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<Species>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Scraper/Definitions")]
        public IActionResult GetAllDefinitions()
        {
            try
            {
                _logger.LogInformation("GET /v1/Scraper called");
                List<IScraperDefinition> definitions = _webScraperService.GetDefinitions();
                return new OkObjectResult(definitions);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Species endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [ProducesResponseType(typeof(Species), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Scraper/Scrape/")]
        public IActionResult ScrapeSpecies([FromBody] Species speciesSource)
        {
            try
            {
                string website = speciesSource.Website;
                Uri websiteUri = new Uri(website);
                _logger.LogInformation($"GET /v1/Scraper/Scrape/ called");
                _logger.LogInformation($"- Resource: {website}");

                var definition = _webScraperService.GetDefinitions().Where(d => d.Host == websiteUri.Host).First();
                var species = new Species();
                definition.ApplyToSpecies(websiteUri, species);

                return new OkObjectResult(new ScrapedSpeciesResponse()
                {
                    Resource = website,
                    Species = species
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Species endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}