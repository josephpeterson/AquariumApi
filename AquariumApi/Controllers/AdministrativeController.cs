using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AquariumApi.Core;
using AquariumApi.Models.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AquariumApi.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministrativeController : Controller
    {
        private readonly IConfiguration _config;
        public readonly IAquariumService _aquariumService;
        private readonly IAdministrativeService _administrativeService;
        public readonly ILogger<SnapshotController> _logger;
        private readonly IAzureService _azureService;
        private readonly IAccountService _accountService;
        private readonly INotificationService _notificationService;

        public AdministrativeController(IConfiguration config,
            IAquariumService aquariumService,
            IAdministrativeService administrativeService,
            ILogger<SnapshotController> logger,
            IAzureService azureService,
            IAccountService accountService,
            INotificationService notificationService)
        {
            _config = config;
            _aquariumService = aquariumService;
            _administrativeService = administrativeService;
            _logger = logger;
            _azureService = azureService;
            _accountService = accountService;
            _notificationService = notificationService;
        }
        [HttpGet]
        [Route("/v1/admin/ApplicationLog")]
        public IActionResult GetApplicationLog()
        {
            try
            {
                var text = System.IO.File.ReadAllText(_config["DashboardLogFilePath"]);
                return new OkObjectResult(text);
            }
            catch (FileNotFoundException)
            {
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        }
        [HttpDelete]
        [Route("/v1/admin/ApplicationLog")]
        public IActionResult DeleteApplicationLog()
        {
            try
            {
                System.IO.File.Delete(_config["DashboardLogFilePath"]);
                return new OkResult();
            }
            catch (FileNotFoundException)
            {
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        }


        [HttpGet]
        [Route(AquariumApiEndpoints.ADMIN_RETRIEVE_ACCOUNTS)]
        public IActionResult GetAquariumUsers()
        {
            try
            {
                var accounts = _administrativeService.GetAquariumUsers();
                return new OkObjectResult(accounts);
            }
            catch
            {
                return new BadRequestResult();
            }
        }
        [HttpGet]
        [Route(AquariumApiEndpoints.ADMIN_RETRIEVE_BUGS)]
        public IActionResult GetBugList()
        {
            try
            {
                var reports = _administrativeService.GetBugReports();
                return new OkObjectResult(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Admin/Bugs endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }


        [HttpGet]
        [Route("/v1/admin/Test")]
        public IActionResult TestMethod()
        {
            try
            {
                var data = System.IO.File.ReadAllBytes("config.json");
                //_azureService.UploadFileToStorage(data,"test.json");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Admin/Bugs endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }


        [HttpGet]
        [Route(AquariumApiEndpoints.ADMIN_RETRIEVE_NOTIFICATIONS)]
        public IActionResult GetNotifications()
        {
            try
            {
                var notifications = _notificationService.GetAllDispatchedNotifications();
                return new OkObjectResult(notifications);
            }
            catch
            {
                return new BadRequestResult();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.ADMIN_NOTIFICATION_DELETE_ALL)]
        public IActionResult DeleteNotification([FromBody] List<int> notificationIds)
        {
            try
            {
                _notificationService.DeleteDispatchedNotifications(notificationIds);
                return new OkResult();
            }
            catch
            {
                return new BadRequestResult();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.ADMIN_NOTIFICATION_DISPATCH)]
        public IActionResult EmitNotification([FromBody] NotificationDispatchRequest req)
        {
            try
            {
                _logger.LogInformation($"POST /v1/admin/Notification endpoint called");
                var notification = req.Notification;
                var id = _accountService.GetCurrentUserId();
                notification.DispatcherId = id;
                notification.Date = DateTime.Now.ToUniversalTime();
                if (req.AccountIds != null)
                {
                    _logger.LogInformation($"Dispatching notification to {req.AccountIds.Count()} accounts");
                    _notificationService.EmitAsync(notification, req.AccountIds).Wait();
                }
                else
                {
                    _logger.LogInformation($"Dispatching notification to all accounts");
                    _notificationService.EmitAsync(notification).Wait();
                }
                return new OkResult();
            }
            catch(Exception ex)
            {
                _logger.LogError($"POST /v1/admin/Notification endpoint caught exception: { ex.Message } Details: { ex.ToString() }");

                return new BadRequestResult();
            }
        }
        [HttpPost]
        [Route(AquariumApiEndpoints.ADMIN_NOTIFICATION_DISMISS)]
        public IActionResult DismissNotifications([FromBody] List<int> notificationIds)
        {
            try
            {
                _logger.LogInformation($"POST /v1/admin/Notification/Dismiss endpoint called");
                _notificationService.DismissDispatchedNotifications(notificationIds);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/admin/Notification/Dismiss endpoint caught exception: { ex.Message } Details: { ex.ToString() }");

                return new BadRequestResult();
            }
        }
    }
}