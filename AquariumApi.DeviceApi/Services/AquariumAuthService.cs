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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IAquariumAuthService
    {
        void Setup(Action setupCallback, Action cleanUpCallback);
        DeviceLoginResponse GetToken();
        Task<DeviceLoginResponse> AttemptLogin(DeviceLoginRequest loginRequest);
        Task RenewAuthenticationToken();
        void Logout();
        void ApplyAquariumDeviceFromService(AquariumDevice aquariumDevice);
        Aquarium GetAquarium();
        AquariumUser GetAccount();
        
        [System.Obsolete]
        void RequestAquariumDeviceFromService();
    }
    public class AquariumAuthService : IAquariumAuthService
    {
        private IConfiguration _config;
        private ILogger<AquariumAuthService> _logger;
        private DeviceLoginResponse _token;
        private Action _bootstrapSetup;
        private Action _bootstrapCleanup;

        public CancellationTokenSource RenewTokenTick { get; private set; }

        public AquariumAuthService(IConfiguration config, ILogger<AquariumAuthService> logger)
        {
            _config = config;
            _logger = logger;
        }
        private void SaveTokenToCache(DeviceLoginResponse deviceLoginResponse)
        {
            _token = deviceLoginResponse;
            File.WriteAllText("login.json", JsonConvert.SerializeObject(deviceLoginResponse));
        }
        private DeviceLoginResponse LoadTokenFromCache()
        {
            if (!File.Exists("login.json")) return null;

            _token = JsonConvert.DeserializeObject<DeviceLoginResponse>(File.ReadAllText("login.json"));
            return _token;
        }
        public DeviceLoginResponse GetToken()
        {
            if (_token != null)
                return _token;
            return LoadTokenFromCache();
        }


        public async Task RenewAuthenticationToken()
        {
            if (_token == null)
                throw new Exception("No authentication token available");

            _logger.LogInformation("Renewing authentication token...");
            var path = $"/v1/Auth/Renew";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_config["AquariumServiceUrl"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.Token);
                client.Timeout = TimeSpan.FromMinutes(5);

                var result = await client.GetAsync(path);
                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception("Could not renew authentication token");
                }

                _logger.LogInformation("Authentication Token Successfully Renewed");

                var res = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<DeviceLoginResponse>(res);
                SaveTokenToCache(response);
                ScheduleRenewToken();
            }
    
        }
        private void ScheduleRenewToken()
        {
            if (RenewTokenTick != null && !RenewTokenTick.IsCancellationRequested)
                RenewTokenTick.Cancel();
            var renewTime = Convert.ToInt32(_config["AuthTokenRenewTime"]);
            var delay = TimeSpan.FromHours(renewTime);
            RenewTokenTick = new CancellationTokenSource();
            CancellationToken ct = RenewTokenTick.Token;
            _ = Task.Run(() =>
            {
                string time = string.Format("{0:D2}h:{1:D2}m", delay.Hours, delay.Minutes);
                _logger.LogInformation($"Renewing auth token in {time}");
                Thread.Sleep(delay);
                if (ct.IsCancellationRequested)
                    return;
                RenewAuthenticationToken().Wait();
            }).ConfigureAwait(false);//Fire and forget
        }
        /* Attempt to retrieve login token */
        /* First attempt will log user with no aquarium */
        /* Second attempt will select an aquarium */
        public async Task<DeviceLoginResponse> AttemptLogin(DeviceLoginRequest loginRequest)
        {
            var path = $"/v1/Auth/Login/Device";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_config["AquariumServiceUrl"]);
                client.Timeout = TimeSpan.FromMinutes(5);

                var result = await client.PostAsJsonAsync(path, loginRequest);
                if (!result.IsSuccessStatusCode)
                {
                    if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                        throw new Exception("Invalid request");
                    if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        throw new Exception("Service does not have this device key on record. Please check your device key and IP combination");
                    else
                        throw new Exception(result.ReasonPhrase);
                }
                var res = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<DeviceLoginResponse>(res);
                SaveTokenToCache(response);

                if (loginRequest.AquariumId.HasValue)
                    _bootstrapSetup();

                //Schedule a renew
                ScheduleRenewToken();
                return response;
            }
        }

        public void Logout()
        {
            _token = null;
            File.Delete("login.json");
            _bootstrapCleanup();
        }

        public void Setup(Action setupCallback, Action cleanUpCallback)
        {
            _bootstrapSetup = setupCallback;
            _bootstrapCleanup = cleanUpCallback;
        }

        public void ApplyAquariumDeviceFromService(AquariumDevice aquariumDevice)
        {
            _token.Aquarium.Device = aquariumDevice;
            SaveTokenToCache(_token);
            _bootstrapSetup();
        }
        public void RequestAquariumDeviceFromService()
        {
            var path = $"/v1/DeviceInteraction";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_config["AquariumServiceUrl"]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.Token);
                client.Timeout = TimeSpan.FromMinutes(5);

                var result = client.GetAsync(path).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception(result.ReasonPhrase);
                var res = result.Content.ReadAsStringAsync().Result;
                var response = JsonConvert.DeserializeObject<AquariumDevice>(res);
                ApplyAquariumDeviceFromService(response);
            }
        }
        public Aquarium GetAquarium()
        {
            return _token?.Aquarium;
        }
        public AquariumUser GetAccount()
        {
            return _token?.Account;
        }
    }
}
