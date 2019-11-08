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
        private readonly IAccountService _accountService;
        private readonly IFishService _fishService;
        public readonly ILogger<FishController> _logger;
        public FishController(IAquariumService aquariumService,
            IFishService fishService,
            IAccountService accountService,
            ILogger<FishController> logger)
        {
            _aquariumService = aquariumService;
            _accountService = accountService;
            _fishService = fishService;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(typeof(Fish), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Fish/{fishId}")]
        public IActionResult GetFishById(int fishId)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Fish/{fishId} called");
                return new OkObjectResult(_fishService.GetFishById(fishId));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Fish/{fishId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<Fish>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        [Route("/v1/Fish")]
        public IActionResult GetAllFish()
        {
            try
            {
                _logger.LogInformation($"GET /v1/Fish called");
                return new OkObjectResult(_fishService.GetAllFishByAccount(_accountService.GetCurrentUserId()));
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Fish endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
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
                //Access level
                var id = _accountService.GetCurrentUserId();
                var aq = _aquariumService.GetAquariumById(fish.AquariumId);
                var access = _accountService.CanModify(id, aq);
                if (!access) return Unauthorized();

                _logger.LogInformation("POST /v1/Fish/Add called");
                var newFish = _fishService.CreateFish(fish);
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
                //Access level
                var id = _accountService.GetCurrentUserId();
                var access = _accountService.CanModify(id,fish);
                if(!access) return Unauthorized();

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
                //Access level
                var id = _accountService.GetCurrentUserId();
                var fish = _fishService.GetFishById(fishId);
                var access = _accountService.CanModify(id, fish);
                if (!access) return Unauthorized();

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

        [HttpPost, Route("/v1/Fish/Death")]
        public IActionResult MarkFishDeath([FromBody] FishDeath death)
        {
            try
            {
                //Access level
                var id = _accountService.GetCurrentUserId();
                var fish = _fishService.GetFishById(death.FishId);
                var access = _accountService.CanModify(id, fish);
                if (!access) return Unauthorized();

                _logger.LogInformation($"POST /v1/Fish/Death called");
                var d = _fishService.MarkDeseased(death);
                return new OkObjectResult(d);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/Death endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost, Route("/v1/Fish/Breed")]
        public IActionResult BreedFish([FromBody] FishBreeding breeding)
        {
            try
            {
                //Access level
                var id = _accountService.GetCurrentUserId();
                var m = _fishService.GetFishById(breeding.MotherId);
                var d = _fishService.GetFishById(breeding.FatherId);
                var access = _accountService.CanModify(id, m);
                var access2 = _accountService.CanModify(id, d);
                if (!access || !access2) return Unauthorized();

                _logger.LogInformation($"POST /v1/Fish/Breed called");
                var breed = _fishService.Breed(breeding);
                return new OkObjectResult(breed);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/Breed endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost, Route("/v1/Fish/Transfer/{aquariumId}")]
        public IActionResult TransferFish(int aquariumId,[FromBody] Fish f)
        {
            try
            {
                //Access level
                var id = _accountService.GetCurrentUserId();
                var fish = _fishService.GetFishById(f.Id);
                var access = _accountService.CanModify(id, fish);
                if (!access) return Unauthorized();

                _logger.LogInformation($"POST /v1/Fish/Transfer/{aquariumId} called");
                var d = _fishService.TransferFish(fish.Id,aquariumId);
                return new OkObjectResult(d);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/Transfer/{aquariumId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost, Route("/v1/Fish/Disease")]
        public IActionResult AddDiseaseDiagnosis([FromBody] FishDisease f)
        {
            try
            {
                //Access level
                var id = _accountService.GetCurrentUserId();
                var fish = _fishService.GetFishById(f.FishId);
                var access = _accountService.CanModify(id, fish);
                if (!access) return Unauthorized();

                _logger.LogInformation($"POST /v1/Fish/Disease called");
                var d = _fishService.AddDiseaseDiagnosis(f);
                return new OkObjectResult(d);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/Disease endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Fish/{fishId}/UpdateThumbnail")]
        public IActionResult UpdateThumbnail(int fishId,[FromBody] PhotoContent photo)
        {
            try
            {
                //Access level
                var id = _accountService.GetCurrentUserId();
                var fish = _fishService.GetFishById(fishId);
                var access = _accountService.CanModify(id, fish);
                if (!access) return Unauthorized();
                //todo check if user has access to this photo

                _logger.LogInformation("POST /v1/Fish/{fishId}/UpdateThumbnail called");
                fish.ThumbnailPhotoId = photo.Id;
                fish=  _aquariumService.UpdateFish(fish);
                return new OkObjectResult(fish);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/{fishId}/UpdateThumbnail endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}