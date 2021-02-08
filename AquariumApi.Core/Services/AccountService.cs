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
        string IssueUserLoginToken(AquariumUser aquariumUser);
        int GetCurrentUserId();
        void SendResetPasswordEmail(string email);
        string UpgradePasswordResetToken(string token);
        AquariumUser AttemptPasswordReset(string requestToken, string newPassword);
        bool CanAccess(int accountId, AquariumUser user);
        bool CanAccess(int accountId, Aquarium aquarium);
        bool CanModify(int accountId, Aquarium aquarium);

        bool CanModify(int accountId, Fish fish);
        bool CanAccess(int accountId, Fish fish);
        AquariumProfile UpdateProfile(AquariumProfile profile);
        string IssueDeviceLoginToken(AquariumUser aquariumUser,int? aquariumId);
        int GetCurrentAquariumId();
        string GetCurrentUserType();
        AquariumUser AttemptUserCredentials(string email, string password);
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
        public string GetCurrentUserType()
        {
            return _context.HttpContext.User.FindFirst(ClaimTypes.Role).Value;
        }
        public int GetCurrentUserId()
        {
            try
            {
                return Convert.ToInt16(_context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch
            {
                return -1;
            }
        }
        public int GetCurrentAquariumId()
        {
            return Convert.ToInt16(_context.HttpContext.User.FindFirst(ClaimTypes.Name).Value);
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
                SeniorityDate = DateTime.Now.ToUniversalTime(),
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
        public AquariumProfile UpdateProfile(AquariumProfile profile)
        {
            return _aquariumDao.UpdateProfile(profile);
        }
        public AquariumUser UpdateUser(AquariumUser user)
        {
            return _aquariumDao.UpdateUser(user);
        }
        public void DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }
        public AquariumUser AttemptUserCredentials(string email, string password)
        {
            var targetUser = _aquariumDao.GetUserByUsernameOrEmail(email);
            if (targetUser == null)
                throw new UnauthorizedAccessException();

            var hash = _encryptionService.Encrypt(password);
            return _aquariumDao.GetAccountByLogin(targetUser.Id, hash);
        }
        public string IssueUserLoginToken(AquariumUser aquariumUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, aquariumUser.Id.ToString()),
                new Claim(ClaimTypes.Email, aquariumUser.Email),
                new Claim(ClaimTypes.Role, aquariumUser.Role),
                new Claim(ClaimTypes.Name, aquariumUser.Username)
            };
            return GenerateLoginToken(claims);
        }
        public string IssueDeviceLoginToken(AquariumUser aquariumUser, int? aquariumId = null)
        {
            if(aquariumId.HasValue)
            {
                var aqId = Convert.ToInt16(aquariumId);
                var aquarium = _aquariumDao.GetAquariumById(aqId);
                if(aquarium.OwnerId != aquariumUser.Id)
                    throw new UnauthorizedAccessException("You do not own this aquarium");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, aquariumUser.Id.ToString()),
                new Claim(ClaimTypes.Role, "Device"),
                new Claim(ClaimTypes.Name, aquariumId.ToString()),
            };
            return GenerateLoginToken(claims, false);
        }

        private string GenerateLoginToken(List<Claim> claims,bool expires = true)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            DateTime? expirationDate;
            if (expires)
                expirationDate = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:LengthMins"]));
            else
                expirationDate = DateTime.Now.AddDays(2);
            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expirationDate,
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
                user.FindFirst(ClaimTypes.NameIdentifier),
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

        public bool CanAccess(int accountId, AquariumUser user)
        {

            throw new NotImplementedException();
        }

        

        public bool CanModify(int accountId, Fish fish)
        {
            var actualFish = _aquariumDao.GetFishById(fish.Id);
            var ownerId = actualFish.Aquarium.OwnerId;
            if (accountId != ownerId) return false;

            return true;
        }

        public bool CanAccess(int accountId, Fish fish)
        {
            return true;
        }

        public bool CanModify(int accountId, Aquarium aquarium)
        {
            var actualAquarium = _aquariumDao.GetAquariumById(aquarium.Id);
            var ownerId = actualAquarium.OwnerId;
            if (accountId != ownerId) return false;

            return true;
        }
        public bool CanAccess(int accountId, Aquarium aquarium)
        {
            return true;
        }
    }
}
