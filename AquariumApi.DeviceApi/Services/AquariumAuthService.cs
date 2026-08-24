using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IAquariumAuthService
    {
        void Setup(Action setupCallback, Action cleanUpCallback);
        Task<DeviceLoginResponse> AttemptLogin(DeviceLoginRequest loginRequest);
        Task RenewAuthenticationToken();
        void Logout();
        Aquarium GetAquarium();
        AquariumUser GetAccount();
    }
    public class AquariumAuthService : IAquariumAuthService
    {
        private ILogger<AquariumAuthService> _logger;
        private readonly IDeviceConfigurationService _deviceConfiguration;
        private DeviceLoginResponse _token;
        private Action _bootstrapSetup;
        private Action _bootstrapCleanup;

        public CancellationTokenSource RenewTokenTick { get; private set; }

        public AquariumAuthService(ILogger<AquariumAuthService> logger
            , IDeviceConfigurationService deviceConfigurationService
            )
        {
            _logger = logger;
            _deviceConfiguration = deviceConfigurationService;
        }
        public async Task<DeviceLoginResponse> AttemptLogin(DeviceLoginRequest loginRequest)
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            var path = AquariumApiEndpoints.AUTH_LOGIN_DEVICE;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(deviceConfiguration.Settings.AquariumServiceUrl);
                client.Timeout = TimeSpan.FromMinutes(5);

                var httpContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(path, httpContent);
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
                _deviceConfiguration.SaveAccountInformation(response);

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
            _deviceConfiguration.SaveAccountInformation(null);
            _bootstrapCleanup();
        }

        public void Setup(Action setupCallback, Action cleanUpCallback)
        {
            _bootstrapSetup = setupCallback;
            _bootstrapCleanup = cleanUpCallback;
        }
        public Aquarium GetAquarium()
        {
            return _token?.Aquarium;
        }
        public AquariumUser GetAccount()
        {
            return _token?.Account;
        }

        public async Task RenewAuthenticationToken()
        {
            if (_token == null)
                throw new Exception("No authentication token available");
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            _logger.LogInformation("Renewing authentication token...");
            var path = AquariumApiEndpoints.AUTH_RENEW;
            var retries = 3;
            using (var client = new HttpClient())
            {
                HttpResponseMessage result = null;
                client.BaseAddress = new Uri(deviceConfiguration.Settings.AquariumServiceUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.Token);
                client.Timeout = TimeSpan.FromMinutes(5);

                for (var i = 0; i < retries; i++)
                {
                    result = await client.GetAsync(path);
                    if (result.IsSuccessStatusCode)
                        break;
                }
                if (result == null || !result.IsSuccessStatusCode)
                    throw new Exception($"Could not renew authentication token after {retries} connection attempts.");


                _logger.LogInformation("Authentication Token Successfully Renewed");

                var res = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<DeviceLoginResponse>(res);
                ScheduleRenewToken();
                //RequestAquariumDeviceFromService();
            }

        }
        private void ScheduleRenewToken()
        {
            var deviceConfiguration = _deviceConfiguration.LoadDeviceConfiguration();
            _logger.LogInformation($"Scheduling auth token renew...");
            if (RenewTokenTick != null && !RenewTokenTick.IsCancellationRequested)
                RenewTokenTick.Cancel();
            int renewTime = deviceConfiguration.Settings.AuthTokenRenewTime;
            var delay = TimeSpan.FromHours(renewTime);
            RenewTokenTick = new CancellationTokenSource();
            CancellationToken ct = RenewTokenTick.Token;
            _ = Task.Run(() =>
            {
                try
                {
                    string time = string.Format("{0:D2}h:{1:D2}m", delay.Hours, delay.Minutes);
                    _logger.LogInformation($"Renewing auth token in {time}");
                    Thread.Sleep(delay);
                    if (ct.IsCancellationRequested)
                        return;
                    RenewAuthenticationToken().Wait();
                }
                catch (Exception e)
                {
                    _logger.LogCritical("Renew Authentication Token: Error Occurred:");
                    _logger.LogCritical(e.Message);
                    _logger.LogCritical(e.StackTrace);
                }

            }).ConfigureAwait(false);//Fire and forget
        }
        /* Attempt to retrieve login token */
        /* First attempt will log user with no aquarium */
        /* Second attempt will select an aquarium */
    }

}
