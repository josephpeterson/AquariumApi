using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi.Clients
{
    public interface IAquariumClient
    {
        AquariumSnapshot SendAquariumSnapshotToHost(AquariumSnapshot snapshot, byte[] photo);
        Task<AquariumDevice> PingAquariumService();
        Task DispatchExceptions(List<BaseException> exceptions);
        Task<ScheduledJob> DispatchScheduledJob(ScheduledJob scheduledJob);
        Task<WaterChange> DispatchWaterChange(WaterChange waterChange);
        Task<ATOStatus> DispatchWaterATO(ATOStatus waterATO);
    }
    public class AquariumClient : IAquariumClient
    {
        private ILogger<IAquariumClient> _logger;
        private IConfiguration _config;
        private IAquariumAuthService _aquariumAuthService;


        public AquariumClient(IConfiguration config,ILogger<IAquariumClient> logger,
            IAquariumAuthService aquariumAuthService)
        {
            _logger = logger;
            _config = config;
            _aquariumAuthService = aquariumAuthService;
        }
        private HttpClient GetHttpClient()
        {
            var token = _aquariumAuthService.GetToken();
            if (token == null)
                throw new Exception("No authentication token available");
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_config["AquariumServiceUrl"]);
            client.Timeout = TimeSpan.FromMinutes(5);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            return client;
        }
        public AquariumSnapshot SendAquariumSnapshotToHost(AquariumSnapshot snapshot, byte[] photo)
        {
            var path = DeviceInboundEndpoints.DISPATCH_SNAPSHOT;
            _logger.LogInformation("Sending snapshot: " + path);


            using (var client = GetHttpClient())
            {
                MultipartFormDataContent multiContent = new MultipartFormDataContent();
                multiContent.Add(new ByteArrayContent(photo), "SnapshotImage", DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());

                //var httpContent = new StringContent(JsonConvert.SerializeObject(snapshot), Encoding.UTF8, "application/json");
                multiContent.Add(new StringContent(JsonConvert.SerializeObject(snapshot)), String.Format("\"{0}\"", "Snapshot"));
                var result = client.PostAsync(path, multiContent).Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not upload aquarium snapshot");
                var actualSnapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(result.Content.ReadAsStringAsync().Result);
                _logger.LogInformation($"Snapshot saved successfully (SnapshotId: {actualSnapshot.Id})");
                return actualSnapshot;
            }
           
        }
        public async Task<AquariumDevice> PingAquariumService()
        {
            var path = DeviceInboundEndpoints.RECEIVE_PING;
            HttpClient client = GetHttpClient();
            var result = await client.GetAsync(path);
            if (!result.IsSuccessStatusCode)
                throw new UnauthorizedAccessException(result.ReasonPhrase);

            var res = await result.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<AquariumDevice>(res);
            return response;
        }
        public async Task DispatchExceptions(List<BaseException> exceptions)
        {
            var path = DeviceInboundEndpoints.DISPATCH_EXCEPTION;
            _logger.LogInformation($"Dispatching {exceptions.Count} exceptions to aquarium service...");

            using (HttpClient client = GetHttpClient())
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(exceptions), Encoding.UTF8, "application/json");
                var result = await client.PostAsync(path, httpContent);
                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception("Could not dispatch exceptions to service");
                }
                _logger.LogInformation($"Exceptions successfully dispatched to service.");
            }
        }
        public async Task<ScheduledJob> DispatchScheduledJob(ScheduledJob scheduledJob)
        {
            var path = DeviceInboundEndpoints.DISPATCH_SCHEDULEDJOB;
            _logger.LogInformation($"Dispatching scheduled job to aquarium service...");

            using (HttpClient client = GetHttpClient())
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(scheduledJob), Encoding.UTF8, "application/json");
                var result = await client.PutAsync(path, httpContent);
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not dispatch scheduled job to service");
                _logger.LogInformation($"Scheduled job successfully dispatched to service.");
                var res = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<ScheduledJob>(res);
                return response;
            }
        }
        public async Task<WaterChange> DispatchWaterChange(WaterChange waterChange)
        {
            var path = DeviceInboundEndpoints.DISPATCH_WATERCHANGE;
            _logger.LogInformation($"Dispatching water change to service...");

            using (HttpClient client = GetHttpClient())
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(waterChange), Encoding.UTF8, "application/json");
                var result = await client.PutAsync(path, httpContent);
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not dispatch water change to service");
                _logger.LogInformation($"Water change successfully dispatched to service.");
                var res = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<WaterChange>(res);
                return response;
            }
        }
        public async Task<ATOStatus> DispatchWaterATO(ATOStatus waterATO)
        {
            var path = DeviceInboundEndpoints.DISPATCH_ATO;
            _logger.LogInformation($"Dispatching water ATO to service...");

            using (HttpClient client = GetHttpClient())
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(waterATO), Encoding.UTF8, "application/json");
                var result = await client.PutAsync(path, httpContent);
                if (!result.IsSuccessStatusCode)
                    throw new Exception("Could not dispatch water ATO to service");
                _logger.LogInformation($"Water ATO successfully dispatched to service.");
                var res = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<ATOStatus>(res);
                return response;
            }
        }

    }

}
