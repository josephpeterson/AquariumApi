using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IAccountService
    {
        AquariumUser AddUser(SignupRequest signupRequest);
        List<AquariumUser> GetAllUsers();
        AquariumUser GetUserById(int id);
        AquariumUser GetUserByUsername(string username);
        AquariumUser GetUserByEmail(string email);
        AquariumUser UpdateUser(AquariumUser user);
        void DeleteUser(int userId);
        string LoginUser(string email, string password);
        int GetCurrentUserId();
        void SendResetPasswordEmail(string email);
        string UpgradePasswordResetToken(string token);
        AquariumUser AttemptPasswordReset(string requestToken, string newPassword);
    }
    public class AccountService : IAccountService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;
        private readonly IEncryptionService _encryptionService;
        private readonly IHttpContextAccessor _context;
        private IEmailerService _emailerService;

        public AccountService(IHttpContextAccessor context,IConfiguration configuration,IAquariumDao aquariumDao,IEncryptionService encryptionService,IEmailerService emailerService)
        {
            _context = context;
            _emailerService = emailerService;
            _configuration = configuration;
            _aquariumDao = aquariumDao;
            _encryptionService = encryptionService;
        }
        public int GetCurrentUserId()
        {
            return Convert.ToInt16(_context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
        public AquariumUser AddUser(SignupRequest signupRequest)
        {
            if (signupRequest.Password != signupRequest.Password2)
                throw new Exception("Passwords do not match.");
            if (_aquariumDao.GetUserByEmail(signupRequest.Email) != null)
                throw new Exception("There is already account with this email.");
            if (_aquariumDao.GetUserByUsername(signupRequest.Username) != null)
                throw new Exception("Sorry, this username is already taken. Please try another one.");

            signupRequest.Password = _encryptionService.Encrypt(signupRequest.Password);
            signupRequest.Account =  new AquariumUser()
            {
                Username = signupRequest.Username,
                Email = signupRequest.Email,
                Role = "User",
                SeniorityDate = DateTime.Now,
                Profile = new AquariumProfile()
            };
            var user = _aquariumDao.AddAccount(signupRequest);
            return user;
        }
        public List<AquariumUser> GetAllUsers()
        {
            return _aquariumDao.GetAllAccounts();
        }
        public AquariumUser GetUserById(int userId)
        {
            return _aquariumDao.GetAccountById(userId);
        }
        public AquariumUser GetUserByEmail(string email)
        {
            return _aquariumDao.GetUserByEmail(email);
        }
        public AquariumUser GetUserByUsername(string username)
        {
            return _aquariumDao.GetUserByUsername(username);

        }
        public AquariumUser UpdateUser(AquariumUser user)
        {
            return _aquariumDao.UpdateUser(user);
        }
        public void DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }
        public string LoginUser(string email, string password)
        {
            var targetUser = _aquariumDao.GetUserByUsernameOrEmail(email);
            if (targetUser == null)
                throw new UnauthorizedAccessException();

            var hash = _encryptionService.Encrypt(password);
            AquariumUser user = _aquariumDao.GetAccountByLogin(targetUser.Id, hash);


            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:LengthMins"])),
                signingCredentials: signinCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
        private string GenerateResetPassword(int uId)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Password:ResetPasswordSecret"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, uId.ToString()),
            };
            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Password:ResetPasswordLimitMins"])),
                signingCredentials: signinCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            var url = _configuration["Password:ResetRedirectUri"] + $"/{token}";
            return url;
        }
        public void SendResetPasswordEmail(string email)
        {
            var user = _aquariumDao.GetUserByUsernameOrEmail(email);
            if (user == null)
                throw new InvalidProgramException();
            email = user.Email;
            var resetLink = GenerateResetPassword(user.Id);

            var subject = "[Aquarium Monitor] Password Reset Requested";
            var content = "You have recently requested to change your password. You may click this link to reset your password\n\n" + resetLink;
            _emailerService.SendAsync(email, subject, content);
        }
        public string UpgradePasswordResetToken(string token)
        {
            TokenValidationParameters validationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Password:ResetPasswordSecret"]))
            };
            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var user = handler.ValidateToken(token, validationParameters, out validatedToken);

            //Generate a new (smaller token)
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Password:ResetPasswordSecret2"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, GetCurrentUserId().ToString()),
            };
            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Password:ResetPasswordLimitMins2"])),
                signingCredentials: signinCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
        public AquariumUser AttemptPasswordReset(string requestToken,string newPassword)
        {
            TokenValidationParameters validationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Password:ResetPasswordSecret2"]))
            };
            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var user = handler.ValidateToken(requestToken, validationParameters, out validatedToken);

            var uId = Convert.ToInt16(user.FindFirst(ClaimTypes.NameIdentifier).Value);

            var pwd = _encryptionService.Encrypt(newPassword);
            _aquariumDao.UpdatePasswordForUser(uId, pwd);
            return GetUserById(uId);
        }
    }
}
