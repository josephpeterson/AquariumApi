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
    public class FeedingController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<SnapshotController> _logger;
        public FeedingController(IAquariumService aquariumService, ILogger<SnapshotController> logger)
        {
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpPost]
        [Route("/v1/Feeding/Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddFeeding([FromBody]Feeding feeding)
        {
            try
            {
                _logger.LogInformation("POST /v1/Feeding/Add called");
                var newFeeding = _aquariumService.AddFeeding(feeding);
                return CreatedAtAction(nameof(GetFeedingById), new { id = newFeeding.Id }, newFeeding);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Feeding/Add caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<Aquarium>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Feeding/{feedingId}")]
        public IActionResult GetFeedingById(int feedingId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Feeding/{feedingId} called");
                var feedings = _aquariumService.GetFeedingById(feedingId);
                return new OkObjectResult(feedings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/{feedingId}/Feedings endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<Aquarium>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/{aquariumId}/Feedings")]
        public IActionResult AllAquariumFeedings(int aquariumId)
        {
            try
            {
                _logger.LogInformation("GET /v1/Aquariums called");
                var feedings = _aquariumService.GetFeedingByAquariumId(aquariumId);
                return new OkObjectResult(feedings);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/{aquariumId}/Feedings endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Feeding/Update")]
        public IActionResult UpdateFeeding([FromBody] Feeding updatedFeeding)
        {
            try
            {
                _logger.LogInformation("POST /v1/Feeding/Update called");
                var feeding = _aquariumService.UpdateFeeding(updatedFeeding);
                return new OkObjectResult(feeding);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Feeding/Update endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Feeding/Delete")]
        public IActionResult DeleteFeeding([FromBody] int removeFeedingId)
        {
            try
            {
                _logger.LogInformation("POST /v1/Feeding/Delete called");
                _aquariumService.DeleteFeeding(removeFeedingId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Feeding/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}