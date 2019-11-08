using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace AquariumApi.Controllers
{
    public class ProfileController : Controller
    {
        private IAccountService _accountService;
        public readonly IAquariumService _aquariumService;
        private readonly IPhotoManager _photoManager;
        public readonly ILogger<PhotoController> _logger;
        public ProfileController(IAquariumService aquariumService, ILogger<PhotoController> logger,IPhotoManager photoManager,
            IAccountService accountService)
        {
            _accountService = accountService;
            _aquariumService = aquariumService;
            _photoManager = photoManager;
            _logger = logger;
        }

        [HttpGet]
        [Route("/v1/Profile/{profileId}")]
        public IActionResult GetAquariumProfile(int profileId)
        {
            try
            {
                AquariumProfile data = _aquariumService.GetProfileById(profileId);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Profile/{profileId}/ endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
        [HttpPost]
        [Route("/v1/Profile/Follow")]
        public IActionResult UpsertFollowUser([FromBody]FollowRequest followRequest)
        {
            try
            {
                AccountRelationship data = _aquariumService.UpsertFollowUser(followRequest.AccountId, followRequest.TargetId);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Profile/Follow endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }

        }
        [HttpPost]
        [Route("/v1/Profile/UpdateThumbnail")]
        public IActionResult UpdateThumbnail([FromBody] PhotoContent photo)
        {
            try
            {
                _logger.LogInformation("POST /v1/Profile/UpdateThumbnail called");

                //Access level
                var id = _accountService.GetCurrentUserId();
                //todo check if user has access to this photo

                var profile = _aquariumService.GetProfileById(id);
                profile.Thumbnail = photo.Id;
                var p = _accountService.UpdateProfile(profile);
                return new OkObjectResult(p);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Profile/UpdateThumbnail endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        public class FollowRequest
        {
            public int AccountId { get; set; }
            public int TargetId { get; set; }
        }
    }
}