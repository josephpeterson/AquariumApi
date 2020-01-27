using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IAquariumAuthService
    {
        void DeleteToken();
        DeviceLoginResponse GetToken();
        DeviceLoginResponse LoadTokenFromCache();
        void SaveTokenToCache(DeviceLoginResponse deviceLoginResponse);
    }
    public class AquariumAuthService : IAquariumAuthService
    {
        private IConfiguration _config;
        private ILogger<AquariumAuthService> _logger;
        private DeviceLoginResponse _token;

        public AquariumAuthService(IConfiguration config, ILogger<AquariumAuthService> logger)
        {
            _config = config;
            _logger = logger;
        }
        public void SaveTokenToCache(DeviceLoginResponse deviceLoginResponse)
        {
            _token = deviceLoginResponse;
            File.WriteAllText("login.json", JsonConvert.SerializeObject(deviceLoginResponse));
        }
        public DeviceLoginResponse LoadTokenFromCache()
        {
            if (!File.Exists("login.json")) return null;

            _token = JsonConvert.DeserializeObject<DeviceLoginResponse>(File.ReadAllText("login.json"));
            return _token;
        }
        public void DeleteToken()
        {
            _token = null;
            File.Delete("login.json");
        }
        public DeviceLoginResponse GetToken()
        {
            if (_token != null)
                return _token;
            return LoadTokenFromCache();
        }
    }
}
