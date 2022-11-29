using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IMixingStationService
    {
        Task<AquariumMixingStation> SearchByHostname(string hostname);
    }
    public class MixingStationService : IMixingStationService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MixingStationService> _logger;

        public MixingStationService(IConfiguration config, ILogger<MixingStationService> logger)
        {
            _config = config;
            _logger = logger;
        }
        public async Task<AquariumMixingStation> SearchByHostname(string hostname)
        {
            if (hostname == null)
                throw new DeviceException("Invalid hostname");
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://" + hostname);
            try
            {
                var res = await client.GetAsync("/status");
                AquariumMixingStation mixingStation = JsonConvert.DeserializeObject<AquariumMixingStation>(res.Content.ReadAsStringAsync().Result);
                mixingStation.Hostname = hostname;
                return mixingStation;
            }
            catch(Exception e)
            {
                throw new DeviceException($"Could not find device by hostname '{hostname}'");
            }
        }
    }
}
