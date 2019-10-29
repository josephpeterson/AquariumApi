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
    }
    public class AccountService : IAccountService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;
        private readonly IEncryptionService _encryptionService;
        private readonly IHttpContextAccessor _context;

        public AccountService(IHttpContextAccessor context,IConfiguration configuration,IAquariumDao aquariumDao,IEncryptionService encryptionService)
        {
            _context = context;
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
    }
}
