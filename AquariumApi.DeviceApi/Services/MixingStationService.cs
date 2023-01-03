using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using AquariumApi.Models.Constants;
using Bifrost.IO.Ports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IMixingStationService
    {
        Task<AquariumMixingStationStatus> PingByHostname(string hostname);
        Task<AquariumMixingStationStatus> PingByHostname();
        Task<List<AquariumMixingStationStatus>> SearchForMixingStation();
        Task<DeviceConfiguration> ConnectToMixingStation(AquariumMixingStationStatus mixingStation);
        void DisconnectFromMixingStation();
        Task<AquariumMixingStationStatus> TestMixingStationValve(int valveId);
        Task<AquariumMixingStationStatus> StopMixingStationSessions();
    }
    public class MixingStationService : IMixingStationService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<MixingStationService> _logger;
        private readonly IDeviceConfigurationService _deviceConfigurationService;

        public MixingStationService(IConfiguration config, ILogger<MixingStationService> logger,IDeviceConfigurationService deviceConfigurationService)
        {
            _config = config;
            _logger = logger;
            _deviceConfigurationService = deviceConfigurationService;
        }

        public async Task<DeviceConfiguration> ConnectToMixingStation(AquariumMixingStationStatus mixingStation)
        {
            //attempt to connect
            var status = await PingByHostname(mixingStation.Hostname);
            var deviceConfiguration = _deviceConfigurationService.LoadDeviceConfiguration();
            deviceConfiguration.MixingStation = new AquariumMixingStation()
            {
                Hostname = mixingStation.Hostname,
                LastConnected = DateTime.Now,
                SensorCount = status.Valves.Count
            };
            _deviceConfigurationService.SaveDeviceConfiguration(deviceConfiguration);
            return deviceConfiguration;
        }

        public void DisconnectFromMixingStation()
        {
            var deviceConfiguration = _deviceConfigurationService.LoadDeviceConfiguration();
            if (deviceConfiguration.MixingStation == null)
                throw new DeviceException("This device is not connected to a mixing station.");
            deviceConfiguration.MixingStation = null;
            _deviceConfigurationService.SaveDeviceConfiguration(deviceConfiguration);
        }

        public async Task<AquariumMixingStationStatus> PingByHostname(string hostname)
        {
            if (hostname == null)
                throw new DeviceException("Invalid hostname");
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://" + hostname);
            try
            {
                var res = await client.GetAsync(MixingStationEndpoints.PING);
                AquariumMixingStationStatus mixingStation = JsonConvert.DeserializeObject<AquariumMixingStationStatus>(res.Content.ReadAsStringAsync().Result);
                if (mixingStation.Hostname == null)
                    mixingStation.Hostname = hostname;
                return mixingStation;
            }
            catch(Exception e)
            {
                throw new DeviceException($"Could not find device by hostname '{hostname}'");
            }
        }
        public async Task<AquariumMixingStationStatus> PingByHostname()
        {
            var mixingStation = _deviceConfigurationService.LoadDeviceConfiguration().MixingStation;
            return await PingByHostname(mixingStation.Hostname);
        }
        public async Task<List<AquariumMixingStationStatus>> SearchForMixingStation()
        {
            var baseAddress= "192.168.1";
            List<AquariumMixingStationStatus> results = new List<AquariumMixingStationStatus>();
            Task[] tasks = new Task[255];
            var count = 0;
            var max = 255;
            for(var i=0;i<max;i++)
            {
                var idx = i;
                tasks[i] = Task.Run(() =>
                {
                    var hostname = $"{baseAddress}.{idx}";
                    _logger.LogInformation($"Checking {hostname}...");
                    Ping ping = new Ping();
                    PingOptions pingOption = new PingOptions(128, true);
                    var reply = ping.Send(hostname,1000);
                    
                    if(reply.Status == IPStatus.Success)
                    {
                        var cl = new HttpClient();
                        try
                        {
                            var res = cl.GetAsync("http://" + hostname + MixingStationEndpoints.PING).Result;
                            var response = JsonConvert.DeserializeObject<AquariumMixingStationStatus>(res.Content.ReadAsStringAsync().Result);
                            response.Hostname = hostname;
                            if (response != null)
                            {
                                results.Add(response);
                            }
                        }
                        catch (Exception ex)
                        {
                            
                        }
                    }
                });
                
            }
            Task.WaitAll(tasks);
            return results;
        }
        public async Task<AquariumMixingStationStatus> TestMixingStationValve(int valveId)
        {
            _logger.LogInformation($"Testing mixing station valve id: {valveId}");
            var mixingStation = _deviceConfigurationService.LoadDeviceConfiguration().MixingStation;
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://" + mixingStation.Hostname);
            var url = MixingStationEndpoints.TEST_VALVE.AggregateParams($"{valveId}");
            var res = await client.GetAsync(url);
            _logger.LogInformation($"GET {url}");
            AquariumMixingStationStatus status = JsonConvert.DeserializeObject<AquariumMixingStationStatus>(res.Content.ReadAsStringAsync().Result);
            return status;
        }
        public async Task<AquariumMixingStationStatus> StopMixingStationSessions()
        {
            var mixingStation = _deviceConfigurationService.LoadDeviceConfiguration().MixingStation;
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://" + mixingStation.Hostname);
            var res = await client.GetAsync(MixingStationEndpoints.STOP);
            AquariumMixingStationStatus status = JsonConvert.DeserializeObject<AquariumMixingStationStatus>(res.Content.ReadAsStringAsync().Result);
            return status;
        }
    }
}
