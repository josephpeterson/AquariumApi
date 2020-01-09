﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AquariumApi.Controllers
{
    [Authorize]
    public class PhotoController : Controller
    {
        private IAccountService _accountService;
        private readonly IFishService _fishService;
        public readonly IAquariumService _aquariumService;
        private readonly IPhotoManager _photoManager;
        public readonly ILogger<PhotoController> _logger;
        public PhotoController(IAquariumService aquariumService,
            ILogger<PhotoController> logger,
            IPhotoManager photoManager,
            IFishService fishService,
            IAccountService accountService)
        {
            _accountService = accountService;
            _fishService = fishService;
            _aquariumService = aquariumService;
            _photoManager = photoManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("/v1/Photo/{photoId}/{size}")]
        public IActionResult GetPhotoContent(int photoId, string size = "")
        {
            return BadRequest();
            try
            {

                /*
                var data = _aquariumService.GetFishPhotoById(photoId);
                string destination = data.Filepath;
                if (size == "medium")
                    destination = Path.GetDirectoryName(data.Filepath) + "/medium/" + Path.GetFileName(data.Filepath);
                else if (size == "small")
                    destination = Path.GetDirectoryName(data.Filepath) + "/thumbnail/" + Path.GetFileName(data.Filepath);
               */
                var b = _photoManager.GetPhoto(photoId);
                return File(b, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Snapshot/Photo/{photoId}/ endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                var path = Directory.GetCurrentDirectory();
                return base.File("~/fish.png", "image/jpeg");
                //return NotFound();
            }

        }
        [HttpPost, DisableRequestSizeLimit]
        [Route("/v1/Fish/{fishId}/UploadPhoto")]
        public IActionResult UploadPhoto(int fishId, IFormFile photoData)
        {
            try
            {
                //Access level
                var id = _accountService.GetCurrentUserId();
                var fish = _fishService.GetFishById(fishId);
                var access = _accountService.CanModify(id, fish);
                if (!access) return Unauthorized();

                _logger.LogInformation($"POST /v1/Fish/{fishId}/UploadPhoto called");

                FishPhoto fishPhoto = _photoManager.AddFishPhoto(fishId, photoData.OpenReadStream());
                return new OkObjectResult(fishPhoto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Fish/{fishId}/UploadPhoto: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpDelete, Route("/v1/Photo/{photoId}/Delete")]
        public IActionResult DeleteFishPhoto(int photoId)
        {
            try
            {
                //todo access
                _logger.LogInformation($"POST /v1/Photo/{photoId}/Delete called");
                _photoManager.DeletePhoto(photoId);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Photo/{photoId}/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }


        [HttpPost]
        [Route("/v1/Photo/Aquarium/Delete")]
        [ProducesResponseType(typeof(Aquarium), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult DeleteAquariumPhotos([FromBody] List<int> photoIds)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Photo/Aquarium/Delete called");


                //Verify access
                var id = _accountService.GetCurrentUserId();
                var validPhotoIds = _aquariumService.GetAquariumPhotosByAccount(id).Select(p => p.Id);
                var delete = photoIds.Where(pId => validPhotoIds.Contains(pId)).ToList();
                _aquariumService.DeleteAquariumPhotos(delete);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Photo/Aquarium/Delete endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Photo/Aquarium/{aquariumId}")]
        [ProducesResponseType(typeof(Aquarium), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetAquariumPhotos(int aquariumId,[FromBody] PaginationSliver pagination)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Photo/Aquarium/{aquariumId} called");

                var id = _accountService.GetCurrentUserId();
                var aq = _aquariumService.GetAquariumById(aquariumId);
                if (aq.OwnerId != id) return new UnauthorizedResult();


                var aquariumPhotos = _aquariumService.GetAquariumPhotos(aquariumId, pagination);
                return new OkObjectResult(aquariumPhotos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Photo/Aquarium/{aquariumId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Photo/Snapshot/{aquariumId}")]
        [ProducesResponseType(typeof(Aquarium), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetAquariumSnapshotPhotos(int aquariumId, [FromBody] PaginationSliver pagination)
        {
            try
            {
                _logger.LogInformation($"GET /v1/Photo/Snapshot/{aquariumId} called");

                var id = _accountService.GetCurrentUserId();
                var aq = _aquariumService.GetAquariumById(aquariumId);
                if (aq.OwnerId != id) return new UnauthorizedResult();


                var aquariumPhotos = _aquariumService.GetAquariumPhotos(aquariumId,pagination);
                return new OkObjectResult(aquariumPhotos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Photo/Snapshot/{aquariumId} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
    }
}