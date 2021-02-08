using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
    public class AuthController : Controller
    {
        private readonly IEncryptionService _encryptionService;
        public readonly IAccountService _accountService;
        private readonly IActivityService _activityService;
        private readonly INotificationService _notificationService;
        private readonly IAquariumService _aquariumService;
        public readonly ILogger<AuthController> _logger;
        public AuthController(IEncryptionService encryptionService,
            IAccountService accountService,
            INotificationService notificationService,
            IAquariumService aquariumService,
            ILogger<AuthController> logger,
            IActivityService activityService)
        {
            _encryptionService = encryptionService;
            _accountService = accountService;
            _activityService = activityService;
            _notificationService = notificationService;
            _aquariumService = aquariumService;
            _logger = logger;
        }
        [Authorize]
        [HttpGet]
        [Route("/v1/Auth/Renew")]
        public IActionResult Renew()
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/Renew called");
                var userId = _accountService.GetCurrentUserId();
                var loginType = _accountService.GetCurrentUserType();

                var user = _accountService.GetUserById(userId);

                if (loginType == "Device")
                {
                    var aquariumId = _accountService.GetCurrentAquariumId();
                    var aquarium = _aquariumService.GetAquariumById(aquariumId);
                    string token = _accountService.IssueDeviceLoginToken(user, aquariumId);
                    aquarium.Device = _aquariumService.GetAquariumDeviceById(aquarium.Device.Id);
                    var res = new DeviceLoginResponse
                    {
                        Account = user,
                        Token = token,
                        AquariumId = aquariumId,
                        Aquarium = aquarium
                    };
                    return new OkObjectResult(res);
                }
                else if (loginType == "User")
                {
                    string token = _accountService.IssueUserLoginToken(user);
                    var res = new
                    {
                        Token = token
                    };
                    return new OkObjectResult(res);
                }
                else return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Auth/Renew endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("/v1/Auth/Login")]
        public IActionResult Login([FromBody]LoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/Login called");
                var user = _accountService.AttemptUserCredentials(loginRequest.Email, loginRequest.Password);
                var token = _accountService.IssueUserLoginToken(user);

                _activityService.RegisterActivity(new LoginAccountActivity()
                {
                    AccountId = user.Id
                });
                return new OkObjectResult(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Auth/Login endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return Unauthorized();
            }
        }
        [HttpPost]
        [Route("/v1/Auth/Login/Device")]
        public IActionResult DeviceLogin([FromBody]DeviceLoginRequest deviceLogin)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/Login/Device called");
                var user = _accountService.AttemptUserCredentials(deviceLogin.Email, deviceLogin.Password);
                var token = _accountService.IssueDeviceLoginToken(user, deviceLogin.AquariumId);


                user.Aquariums = _aquariumService.GetAquariumsByAccountId(user.Id);



                var data = new DeviceLoginResponse
                {
                    Account = user,
                    Token = token,
                    AquariumId = deviceLogin.AquariumId
                };


                if (deviceLogin.AquariumId.HasValue)
                {
                    var aq = user.Aquariums.First(a => a.Id == deviceLogin.AquariumId);

                    //Register account login activity
                    _activityService.RegisterActivity(new DeviceLoginAccountActivity()
                    {
                        AccountId = user.Id
                    });

                    //Dispatch notification stating new login occured
                    _notificationService.EmitAsync(new DispatchedNotification
                    {
                        Date = DateTime.Now.ToUniversalTime(),
                        Type = NotificationTypes.LoginDeviceActivity,
                        DispatcherId = user.Id,
                        Title = "New Device Login",
                        Subtitle = $"Aquarium monitoring device connected to {aq.Name}",
                    }, new List<int>() { user.Id }).Wait();


                    //If the aquarium did not have a device, create a record for it
                    if (aq.Device == null)
                    {
                        var d = new AquariumDevice
                        {
                            Name = "Remote Login",
                            Type = "RaspberryPi",
                            Address = "",
                            Port = "0",
                            PrivateKey = "",
                            AquariumId = aq.Id
                        };
                        aq.Device = _aquariumService.AddAquariumDevice(d);
                    }
                    else
                        aq.Device = _aquariumService.GetAquariumDeviceById(aq.Device.Id);
                    data.Aquarium = aq;
                }
                
                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Auth/Login/Device endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("/v1/Auth/Signup")]
        public IActionResult Signup([FromBody]SignupRequest signupRequest)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/Signup called");
                var user = _accountService.AddUser(signupRequest);
                _activityService.RegisterActivity(new CreateAccountActivity() { AccountId = user.Id });
                return Login(new LoginRequest
                {
                    Email = user.Email,
                    Password = signupRequest.Password2
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Auth/Signup endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Auth/PasswordReset/Attempt")]
        public IActionResult SendPasswordResetEmail([FromBody]TokenRequest token)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/PasswordReset/Attempt called");
                _accountService.SendResetPasswordEmail(token.Token);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Auth/PasswordReset/Attempt endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Auth/PasswordReset/Upgrade")]
        public IActionResult ResetPasswordHandshake([FromBody]TokenRequest token)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/PasswordReset/Upgrade called");
                var requestToken = _accountService.UpgradePasswordResetToken(token.Token);
                return new OkObjectResult(new TokenRequest
                {
                    Token = requestToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Auth/PasswordReset/Upgrade endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
        [HttpPost]
        [Route("/v1/Auth/PasswordReset/Submit")]
        public IActionResult SubmitPasswordResetRequest([FromBody]PasswordResetRequest request)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/PasswordReset/Submit called");
                var user = _accountService.AttemptPasswordReset(request.Token, request.Password);
                return new OkObjectResult(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Auth/PasswordReset/Submit endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }

        /*
        [HttpGet]
        [Route("/v1/Auth/Recrypt")]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult RecryptPasswords()
        {
            try
            {
                var accounts = _accountService.GetAllUsers();
                accounts.ForEach(acc =>
                {
                    acc.Password = _encryptionService.Encrypt(acc.Password);
                    _accountService.UpdateUser(acc);
                });
                return new OkObjectResult(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET /v1/Account/Current endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return NotFound();
            }
        }
        */
    }
    public class TokenRequest
    {
        public string Token { get; set; }
    }
    public class PasswordResetRequest
    {
        public string Token { get; set; }
        public string Password { get; set; }
    }
}