using System;
using System.Collections.Generic;
using System.IO;
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
    public class FishController : Controller
    {
        public readonly IAquariumService _aquariumService;
        public readonly ILogger<FishController> _logger;
        public FishController(IAquariumService aquariumService, ILogger<FishController> logger)
        {
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<Aquarium>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Fish/{fishId}")]
        public IActionResult GetFishById(int fishId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Fish/{fishId} called");
                return new OkObjectResult(_aquariumService.GetFishById(fishId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Fish/{fishId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Fish/Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddFish([FromBody]Fish fish)
        {
            try
            {
                //Check if this is our aquarium
                int userId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (_aquariumService.GetAquariumById(fish.AquariumId).OwnerId != userId)
                    return Unauthorized();

                _logger.LogInformation("POST /v1/Fish/Add called");
                var newFish = _aquariumService.AddFish(fish);
                return CreatedAtAction(nameof(GetFishById),newFish);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/Add caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Fish/Update")]
        public IActionResult UpdateFish([FromBody]Fish fish)
        {
            try
            {
                //Check if this is our fish
                int userId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (_aquariumService.GetFishById(fish.Id).Aquarium.OwnerId != userId)
                    return Unauthorized();

                _logger.LogInformation("POST /v1/Fish/Update called");
                var updatedFish = _aquariumService.UpdateFish(fish);
                return new OkObjectResult(updatedFish);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/Update endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Fish/Delete")]
        public IActionResult DeleteFish([FromBody] int fishId)
        {
            try
            {
                //Check if this is our fish
                int userId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (_aquariumService.GetFishById(fishId).Aquarium.OwnerId != userId)
                    return Unauthorized();

                _logger.LogInformation("POST /v1/Fish/Delete called");
                _aquariumService.DeleteFish(fishId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }


        [HttpPost, DisableRequestSizeLimit]
        [Route("/v1/Fish/{fishId}/UploadPhoto")]
        public IActionResult UploadPhoto(int fishId, IFormFile photoData)
        {
            try
            {
                //Check if this is our fish
                int userId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (_aquariumService.GetFishById(fishId).Aquarium.OwnerId != userId)
                    return Unauthorized();

                _logger.LogInformation($"POST /v1/Fish/{fishId}/UploadPhoto called");
                FishPhoto fishPhoto = _aquariumService.AddFishPhoto(fishId, photoData);
                return CreatedAtAction(nameof(GetFishById), fishPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/{fishId}/UploadPhoto: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost,Route("/v1/Fish/Photo/Delete")]
        public IActionResult DeleteFishPhoto([FromBody] int fishPhotoId)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Fish/Photo/Delete called");
                _aquariumService.DeleteFishPhoto(fishPhotoId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/Photo/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

    }
}