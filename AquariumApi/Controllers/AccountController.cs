using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public readonly IAquariumService _aquariumService;
        private readonly IAccountService _accountService;
        private readonly INotificationService _notificationService;
        public readonly ILogger<AccountController> _logger;
        private readonly IPhotoManager _photoManager;

        public AccountController(IConfiguration config, 
            IAquariumService aquariumService, 
            IDeviceClient deviceService, 
            IAccountService accountService, 
            INotificationService notificationService,
            ILogger<AccountController> logger,
            IPhotoManager photoManager
            )

        {
            _config = config;
            _aquariumService = aquariumService;
            _accountService = accountService;
            _notificationService = notificationService;
            _logger = logger;
            _photoManager = photoManager;
        }
        [HttpGet]
        [Route("/v1/Account/{id}")]
        [ProducesResponseType(typeof(AquariumUser), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetDetailedAccount(int id)
        {
            try
            {
                int userId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var data = _aquariumService.GetAccountDetailed(userId,id);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Account/{id} endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Account/Claims")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetOurDetails()
        {
            try
            {
                int userId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var data = _aquariumService.GetAccountDetailed(userId, userId);
                return new OkObjectResult(new
                {
                    AquariumUser = data,
                    Claims = User.Claims.ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Account/Claims endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Account/Current")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetCurrentUser()
        {
            try
            {
                int userId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var data = _aquariumService.GetAccountDetailed(userId, userId);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Account/Current endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpGet]
        [Route("/v1/Account/Notifications")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetNotifications()
        {
            try
            {
                int id = _accountService.GetCurrentUserId();
                var data = _notificationService.GetNotificationsByAccountId(id);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Account/Notifications endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        [HttpPost]
        [Route("/v1/Account/Notifications/Dismiss")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult DismissNotifications([FromBody] List<int> notificationIds)
        {
            try
            {
                int id = _accountService.GetCurrentUserId();
                var notifications = _notificationService.GetNotificationsById(notificationIds)
                    .Where(n => n.TargetId == id && !n.Dismissed)
                    .Select(n => n.Id)
                    .ToList();
                _notificationService.DismissNotifications(notifications);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Account/Notifications endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }


        [HttpPut]
        [Route("/v1/Account/Update")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult UpdateAccount([FromBody] AquariumUser account)
        {
            try
            {
                int userId = Convert.ToInt16(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var data = _aquariumService.GetAccountDetailed(userId, userId);
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Account/Current endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }

    }
}