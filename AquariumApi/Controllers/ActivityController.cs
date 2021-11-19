using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class ActivityController : Controller
    {
        private readonly IConfiguration _config;
        public readonly IAquariumService _aquariumService;
        private readonly IActivityService _activityService;
        public readonly ILogger<AccountController> _logger;
        private readonly IPhotoManager _photoManager;

        public ActivityController(IConfiguration config, IActivityService activityService,IAquariumService aquariumService, IDeviceClient deviceService, ILogger<AccountController> logger,IPhotoManager photoManager)
        {
            _config = config;
            _aquariumService = aquariumService;
            _activityService = activityService;
            _logger = logger;
            _photoManager = photoManager;
        }
        [HttpGet]
        [Route(AquariumApiEndpoints.ACCOUNT_RETRIEVE_ACTIVITY)]
        [ProducesResponseType(typeof(AquariumUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public IActionResult GetAccountActivity(int activityId)
        {
            try
            {
                int userId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var data = _activityService.GetActivity(activityId);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET {AquariumApiEndpoints.ACCOUNT_RETRIEVE_ACTIVITY.AggregateParams($"{activityId}")} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
    }
}