using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
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
                _logger.LogInformation("POST /v1/Fish/Add called");
                var newFish = _aquariumService.AddFish(fish);
                return CreatedAtAction(nameof(GetFishById),newFish);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Species/Add caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Fish/Update")]
        public IActionResult UpdateFish([FromBody]Fish fish)
        {
            try
            {
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

                _logger.LogInformation($"POST /v1/Fish/{fishId}/UploadPhoto called");
                FishPhoto fishPhoto = _aquariumService.AddFishPhoto(fishId, photoData);
                return new OkObjectResult(fishPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/{fishId}/UploadPhoto: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("/v1/Fish/Photo/{photoId}/{size}")]
        public IActionResult GetSnapshotPhoto(int photoId, string size = "")
        {
            try
            {
                var data = _aquariumService.GetFishPhotoById(photoId);

                byte[] b;
                if (size == "medium")
                {
                    var destination = Path.GetDirectoryName(data.Filepath) + "/medium/" + Path.GetFileName(data.Filepath);
                    b = System.IO.File.ReadAllBytes(destination);
                }
                else if (size == "small")
                {
                    var destination = Path.GetDirectoryName(data.Filepath) + "/thumbnail/" + Path.GetFileName(data.Filepath);
                    b = System.IO.File.ReadAllBytes(destination);
                }
                else
                    b = System.IO.File.ReadAllBytes(data.Filepath);

                return File(b, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Snapshot/Photo/{photoId}/ endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
    }
}