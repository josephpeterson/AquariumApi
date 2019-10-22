using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
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
    public interface IEncryptionService
    {
        string Encrypt(string key);

    }
    public class EncryptionService : IEncryptionService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;

        public EncryptionService(IConfiguration configuration,IAquariumDao aquariumDao)
        {
            _configuration = configuration;
            _aquariumDao = aquariumDao;
        }
        public string Encrypt(string key)
        {
            var salt = _configuration["EncryptionSalt"];
            byte[] data = Encoding.ASCII.GetBytes(key + salt);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            return Encoding.ASCII.GetString(data);
        }
    }
}
