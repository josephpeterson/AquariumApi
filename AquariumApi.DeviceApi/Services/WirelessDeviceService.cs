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
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IWirelessDeviceService
    {
        DeviceConfiguration UpsertWirelessDevice(WirelessDevice wirelessDeviceStatus);
        DeviceConfiguration RemoveWirelessDevice(WirelessDevice wirelessDevice);
        Task<List<WirelessDeviceStatus>> PingByHostname(List<WirelessDevice> hostnames);
        Task<List<WirelessDevice>> SearchForDevices();
        Task<WirelessDeviceStatus> StopWirelessDevice(WirelessDevice wirelessDevice);
        Task<WirelessDeviceStatus> TriggerWirelessDeviceSensor(WirelessDevice wirelessDevice, int valveId, GpioPinValue gpioPinValue = GpioPinValue.Low);
        Task<List<WirelessDeviceStatus>> GetWirelessDeviceStatuses();
    }
    public class WirelessDeviceService : IWirelessDeviceService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<WirelessDeviceService> _logger;
        private readonly IDeviceConfigurationService _deviceConfigurationService;

        public WirelessDeviceService(IConfiguration config, ILogger<WirelessDeviceService> logger,IDeviceConfigurationService deviceConfigurationService)
        {
            _config = config;
            _logger = logger;
            _deviceConfigurationService = deviceConfigurationService;
        }

        public DeviceConfiguration UpsertWirelessDevice(WirelessDevice wirelessDevice)
        {
            var deviceConfiguration = _deviceConfigurationService.LoadDeviceConfiguration();
            if (!wirelessDevice.Id.HasValue)
            {
                var r = new Random();
                int? v = null;
                while (v == null || deviceConfiguration.Schedules.Any(tt => tt.Id == v))
                    v = r.Next(1000, 9999);
                wirelessDevice.Id = v;
            }

            //Validate
            if (string.IsNullOrEmpty(wirelessDevice.Name))
                throw new DeviceException($"Wireless device must contain a valid name.");
            if (string.IsNullOrEmpty(wirelessDevice.Hostname))
                throw new DeviceException($"Wireless device must contain a valid hostname.");

            var wirelessDevices = deviceConfiguration.WirelessDevices.Where(t => t.Id != wirelessDevice.Id && t.Id.HasValue).ToList();
            deviceConfiguration.WirelessDevices.Add(wirelessDevice);
            _deviceConfigurationService.SaveDeviceConfiguration(deviceConfiguration);
            return deviceConfiguration;
        }

        public DeviceConfiguration RemoveWirelessDevice(WirelessDevice wirelessDevice)
        {
            var deviceConfiguration = _deviceConfigurationService.LoadDeviceConfiguration();
            deviceConfiguration.WirelessDevices = deviceConfiguration.WirelessDevices.Where(wd => wd.Hostname != wirelessDevice.Hostname).ToList();
            _deviceConfigurationService.SaveDeviceConfiguration(deviceConfiguration);
            return deviceConfiguration;
        }
        public async Task<List<WirelessDeviceStatus>> GetWirelessDeviceStatuses()
        {
            var deviceConfiguration = _deviceConfigurationService.LoadDeviceConfiguration();
            return await PingByHostname(deviceConfiguration.WirelessDevices);
        }
        public async Task<List<WirelessDeviceStatus>> PingByHostname(List<WirelessDevice> hostnames)
        {
            return hostnames.Select(wd =>
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://" + wd.Hostname);
                try
                {
                    var res = client.GetAsync(MixingStationEndpoints.PING).Result;
                    WirelessDeviceStatus mixingStation = JsonConvert.DeserializeObject<WirelessDeviceStatus>(res.Content.ReadAsStringAsync().Result);
                    if (mixingStation.Hostname == null)
                        mixingStation.Hostname = wd.Hostname;
                    return mixingStation;
                }
                catch (Exception e)
                {
                    throw new DeviceException($"Could not find device by hostname '{wd.Hostname}'");
                }
            }).ToList();
        }
        public async Task<List<WirelessDevice>> SearchForDevices()
        {
            var wirelessDevices = _deviceConfigurationService.LoadDeviceConfiguration().WirelessDevices;

            var baseAddress= "192.168.1";
            List<WirelessDevice> results = new List<WirelessDevice>();
            var max = 255;
            max = 1;
            Task[] tasks = new Task[max];
            var count = 0;
            for(var i=0;i<max;i++)
            {
                var idx = i;
                tasks[i] = Task.Run(() =>
                {
                    idx = 72;
                    var hostname = $"{baseAddress}.{idx}";
                    _logger.LogInformation($"Checking {hostname}...");
                    Ping ping = new Ping();
                    PingOptions pingOption = new PingOptions(128, true);
                    var reply = ping.Send(hostname, 1000);

                    if (reply.Status == IPStatus.Success)
                    {
                        var cl = new HttpClient();
                        try
                        {
                            var res = cl.GetAsync("http://" + hostname + MixingStationEndpoints.PING).Result;
                            if (!res.IsSuccessStatusCode)
                                return;
                            var responseBody = res.Content.ReadAsStringAsync().Result;
                            var response = JsonConvert.DeserializeObject<WirelessDeviceStatus>(responseBody);
                            response.Hostname = hostname;
                            if (response != null)
                            {
                                var existingDevice = wirelessDevices.FirstOrDefault(x => x.Hostname == response.Hostname);
                                if (existingDevice == null)
                                    existingDevice = new WirelessDevice
                                    {
                                        Name = hostname,
                                        Hostname = hostname,
                                    };
                                existingDevice.Status = response;
                                existingDevice.Sensors = response.Valves;
                                existingDevice.LastConnected = DateTime.Now;
                                results.Add(existingDevice);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"Could not parse response from device. Exception: " + ex.Message + "\r\n" + ex.StackTrace);
                        }
                    }
                });
            }
            Task.WaitAll(tasks);
            return results;
        }
        public async Task<WirelessDeviceStatus> StopWirelessDevice(WirelessDevice wirelessDevice)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://" + wirelessDevice.Hostname);
            var res = await client.GetAsync(MixingStationEndpoints.STOP);
            WirelessDeviceStatus status = JsonConvert.DeserializeObject<WirelessDeviceStatus>(res.Content.ReadAsStringAsync().Result);
            return status;
        }
        public async Task<WirelessDeviceStatus> TriggerWirelessDeviceSensor(WirelessDevice wirelessDevice, int valveId,GpioPinValue gpioPinValue = GpioPinValue.Low)
        {
            _logger.LogInformation($"Testing sensor ID: {valveId} on wireless device Hostname: {wirelessDevice.Hostname}");
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://" + wirelessDevice.Hostname);
            var url = MixingStationEndpoints.TEST_VALVE.AggregateParams($"{valveId}");
            var res = await client.GetAsync(url);
            _logger.LogInformation($"GET {url}");
            var content = await res.Content.ReadAsStringAsync();
            WirelessDeviceStatus status = JsonConvert.DeserializeObject<WirelessDeviceStatus>(content);
            return status;
        }
    }
}
