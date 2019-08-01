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
        public readonly IAccountService _accountService;
        public readonly ILogger<AuthController> _logger;
        public AuthController(IAccountService accountService, ILogger<AuthController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost]
        [Route("/v1/Auth/Login")]
        public IActionResult Login([FromBody]LoginModel user)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/Login called");
                var token = _accountService.LoginUser(user.Email,user.Password);
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
        public IActionResult Signup([FromBody]SignupModel signupRequest)
        {
            try
            {
                _logger.LogInformation($"POST /v1/Auth/Signup called");

                //Form validation
                if (signupRequest.Password != signupRequest.Password2)
                    return BadRequest();

                var user = new AquariumUser();
                user.Email = signupRequest.Email;
                user.Password = signupRequest.Password;
                user.SeniorityDate = new DateTime();

                _accountService.AddUser(user);

                return Login(new LoginModel
                {
                    Email = user.Email,
                    Password = user.Password
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /v1/Auth/Signup endpoint caught exception: { ex.Message } Details: { ex.ToString() }");
                return BadRequest();
            }
        }
    }
}