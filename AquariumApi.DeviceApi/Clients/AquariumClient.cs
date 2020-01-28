using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2.HPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi.Clients
{
    public interface IAquariumClient
    {
        Task<Aquarium> ApplyDeviceHardware(AquariumDevice aquariumDevice);
        AquariumSnapshot SendAquariumSnapshotToHost(string host, AquariumSnapshot snapshot, byte[] photo);
        Task<AquariumUser> RetrieveLoginToken(DeviceLoginRequest loginRequest);
        Task<DeviceLoginResponse> ValidateAuthenticationToken();
        void ClearLoginToken();
    }
    public class AquariumClient: IAquariumClient
    {
        private ILogger<IAquariumClient> _logger;
        private IConfiguration _config;
        private IAquariumAuthService _aquariumAuthService;


        public AquariumClient(ILogger<IAquariumClient> logger, IConfiguration config,
            IAquariumAuthService aquariumAuthService)
        {
            _logger = logger;
            _config = config;
            _aquariumAuthService = aquariumAuthService;
        }


        public AquariumSnapshot SendAquariumSnapshotToHost(string host, AquariumSnapshot snapshot, byte[] photo)
        {
            var path = $"{_config["AquariumServiceUrl"]}/DeviceInteraction/Snapshot";
            _logger.LogInformation("Sending snapshot: " + path);



            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(new ByteArrayContent(photo), "SnapshotImage", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());

            //var httpContent = new StringContent(JsonConvert.SerializeObject(snapshot), Encoding.UTF8, "application/json");
            multiContent.Add(new StringContent(JsonConvert.SerializeObject(snapshot)), String.Format("\"{0}\"","Snapshot"));
            HttpClient client = GetHttpClient();
            var result = client.PostAsync(path, multiContent).Result;
            if (!result.IsSuccessStatusCode)
                throw new Exception("Could not upload aquarium snapshot");
            var actualSnapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(result.Content.ReadAsStringAsync().Result);
            _logger.LogInformation($"Snapshot saved successfully (SnapshotId: {actualSnapshot.Id})");
            return actualSnapshot;
        }

        public async Task<Aquarium> ApplyDeviceHardware(AquariumDevice aquariumDevice)
        {
            var path = $"{_config["AquariumServiceUrl"]}/DeviceInteraction";

            _logger.LogInformation("\n\n** Attempting to apply device hardware ** \n\n");
            _logger.LogInformation("- Service Url: " + path);
            _logger.LogInformation("- Hardware:\n");
            _logger.LogInformation(JsonConvert.SerializeObject(aquariumDevice));

            HttpClient client = GetHttpClient();
            var result = await client.PostAsJsonAsync(path, aquariumDevice);
            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new Exception("Invalid request");
                if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    throw new Exception("Service does not have this device key on record. Please check your device key and IP combination");
            }
            var res = await result.Content.ReadAsStringAsync();
            var aquarium = JsonConvert.DeserializeObject<Aquarium>(res);

            _logger.LogInformation($"- Aquarium: ${aquarium.Name}");
            _logger.LogInformation($"- Aquarium Device: ${aquarium.Device.Name}");
            _logger.LogInformation("Token successfully validated.");
            return aquarium;
        }
        /* Retrieve current token information */
        public async Task<DeviceLoginResponse> ValidateAuthenticationToken()
        {
            var path = $"{_config["AquariumServiceUrl"]}/DeviceInteraction";

            _logger.LogInformation("\n\n** Attempting to validate authentication token ** \n\n");
            _logger.LogInformation("- Service Url: " + path);

            HttpClient client = GetHttpClient();
            var result = await client.GetAsync(path);
      if (!result.IsSuccessStatusCode)
        throw new UnauthorizedAccessException("Could not validate token against server");

            var res = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<DeviceLoginResponse>(res);
            var aquarium = response.Aquarium;
            var account = response.Account;

            _logger.LogInformation($"- Account: {account.Username}");
            _logger.LogInformation($"- Aquarium: {aquarium.Name}");
            _logger.LogInformation($"- Aquarium Device: {aquarium.Device.Name}");
            _logger.LogInformation("Token successfully validated.");
            return response;
        }

        /* Attempt to retrieve login token */
        /* First attempt will log user with no aquarium */
        /* Second attempt will select an aquarium */
        public async Task<AquariumUser> RetrieveLoginToken(DeviceLoginRequest loginRequest)
        {
            //save login information for cache 
            //then on load contact service and validate
            var path = $"{_config["AquariumServiceUrl"]}/Auth/Login/Device";
            _logger.LogInformation("\n\n** Attempting to validate authentication token ** \n\n");
            _logger.LogInformation("- Service Url: " + path);
            if (loginRequest.AquariumId.HasValue)
                _logger.LogInformation($"- Selected Aquarium Id: ${loginRequest.AquariumId}");

            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(5);
            var result = await client.PostAsJsonAsync(path, loginRequest);
            if (!result.IsSuccessStatusCode)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new Exception("Invalid request");
                if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    throw new Exception("Service does not have this device key on record. Please check your device key and IP combination");
            }

            _logger.LogInformation("Initial login sucessful.");

            var res = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<DeviceLoginResponse>(res);
            _aquariumAuthService.SaveTokenToCache(response);
            return response.Account;
        }
        private HttpClient GetHttpClient()
        {
            var token = _aquariumAuthService.GetToken();
            if (token == null)
                throw new Exception("No authentication token available");
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(5);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            return client;
        }
        
        public void ClearLoginToken()
        {
            _aquariumAuthService.DeleteToken();
        }
       
    }
    
}
