using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
    public class AuthController : Controller
    {
        private readonly IEncryptionService _encryptionService;
        public readonly IAccountService _accountService;
        private readonly IActivityService _activityService;
        public readonly ILogger<AuthController> _logger;
        public AuthController(IEncryptionService encryptionService,IAccountService accountService, ILogger<AuthController> logger,IActivityService activityService)
        {
            _encryptionService = encryptionService;
            _accountService = accountService;
            _activityService = activityService;
            _logger = logger;
        }

        [HttpPost]
        [Route("/v1/Auth/Login")]
        public IActionResult Login([FromBody]LoginRequest user)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/Login called");
                var token = _accountService.LoginUser(user.Email,user.Password);

                var userAcc = _accountService.GetUserByUsername(user.Email);
                if(userAcc == null)
                    userAcc = _accountService.GetUserByEmail(user.Email);
                _activityService.RegisterActivity(new LoginAccountActivity() {
                    AccountId = userAcc.Id
                });
                return new OkObjectResult(new { Token = token});
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Auth/Login endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
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
        [Route("/v1/Auth/PasswordReset/Upgrade")]
        public IActionResult ResetPasswordHandshake([FromBody]TokenRequest token)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/PasswordReset/Upgrade called");
                var requestToken = _accountService.UpgradePasswordResetToken(token.Token);
                return new OkObjectResult(new TokenRequest {
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