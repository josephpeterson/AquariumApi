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
        AquariumUser AddUser(AquariumUser user);
        List<AquariumUser> GetAllUsers();
        AquariumUser GetUserById(int id);
        AquariumUser GetUserByUsername(string username);
        AquariumUser GetUserByEmail(string email);
        AquariumUser UpdateUser(AquariumUser user);
        void DeleteUser(int userId);
        string LoginUser(string email, string password);
    }
    public class AccountService : IAccountService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;

        public AccountService(IConfiguration configuration,IAquariumDao aquariumDao)
        {
            _configuration = configuration;
            _aquariumDao = aquariumDao;
        }
        public AquariumUser AddUser(AquariumUser user)
        {
            user.Role = "User";
            user.SeniorityDate = DateTime.Now;
            return _aquariumDao.AddAccount(user);
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
            throw new NotImplementedException();
        }
        public void DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }
        public string LoginUser(string email, string password)
        {
            AquariumUser user = _aquariumDao.GetAccountByLogin(email, password);


            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
    }
}
