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
using System.Drawing;
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
        Task<WirelessDeviceStatus> TriggerWirelessDeviceSensor(WirelessDevice wirelessDevice, List<WirelessDeviceSensor> sensorValues);
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
            if (deviceConfiguration.WirelessDevices.Where(t => t.Id != wirelessDevice.Id).Any(x => x.Hostname == wirelessDevice.Hostname))
                throw new DeviceException($"Hostname is in used by another wireless device.");

            var wirelessDevices = deviceConfiguration.WirelessDevices.Where(t => t.Id != wirelessDevice.Id && t.Id.HasValue).ToList();
            wirelessDevices.Add(wirelessDevice);
            deviceConfiguration.WirelessDevices = wirelessDevices;
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
                var client = new WirelessDeviceClient(wd);
                try
                {
                    var status = client.PingDevice().Result;
                    return status;
                }
                catch (Exception e)
                {
                    _logger.LogWarning($"Could not ping wireless device {wd.Name}: {wd.Hostname}.");
                    return null;
                }
            }).Where(x => x != null).ToList();
        }
        public async Task<List<WirelessDevice>> SearchForDevices()
        {
            var wirelessDevices = _deviceConfigurationService.LoadDeviceConfiguration().WirelessDevices;

            var baseAddress= "192.168.1";
            List<WirelessDevice> results = new List<WirelessDevice>();
            var max = 255;
            Task[] tasks = new Task[max];
            var count = 0;
            for(var i=0;i<max;i++)
            {
                var idx = i;
                tasks[i] = Task.Run(() =>
                {
                    var hostname = $"{baseAddress}.{idx}";
                    //_logger.LogInformation($"Checking {hostname}...");
                    try
                    {
                        Ping ping = new Ping();
                        PingOptions pingOption = new PingOptions(128, true);
                        var reply = ping.Send(hostname, 1000);

                        if (reply.Status == IPStatus.Success)
                        {
                            var client = new WirelessDeviceClient(hostname);
                            var cl = new HttpClient();

                            var response = client.PingDevice().Result;
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
                                existingDevice.Sensors = response.Sensors;
                                existingDevice.LastConnected = DateTime.Now;
                                results.Add(existingDevice);
                            }
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        //ignore
                    }
                    catch (Exception ex)
                    {
                        //_logger.LogWarning($"Could not parse response from {hostname}. Exception: " + ex.Message);
                    }
                });
            }
            await Task.WhenAll(tasks);
            return results;
        }
        public async Task<WirelessDeviceStatus> StopWirelessDevice(WirelessDevice wirelessDevice)
        {
            var client = new WirelessDeviceClient(wirelessDevice);
            WirelessDeviceStatus status = await client.StopWirelessDevice();
            return status;
        }
        public async Task<WirelessDeviceStatus> TriggerWirelessDeviceSensor(WirelessDevice wirelessDevice, int pin,GpioPinValue gpioPinValue = GpioPinValue.Low)
        {
            _logger.LogInformation($"Testing Pin: {pin} on wireless device Hostname: {wirelessDevice.Hostname}");
            var client = new WirelessDeviceClient(wirelessDevice);
            WirelessDeviceStatus status = await client.TriggerWirelessDeviceSensor(pin, gpioPinValue);
            return status;
        }
        public async Task<WirelessDeviceStatus> TriggerWirelessDeviceSensor(WirelessDevice wirelessDevice,List<WirelessDeviceSensor> sensorValues)
        {
            _logger.LogInformation($"Testing multiple pins:\n\t{string.Join("\n\t",sensorValues.Select(p => $"Pin: {p.Pin} Value: {p.Value}").ToList())}\n on wireless device Hostname: {wirelessDevice.Hostname}");
            var client = new WirelessDeviceClient(wirelessDevice);
            WirelessDeviceStatus status = await client.TriggerWirelessDeviceSensor(sensorValues);
            return status;
        }
    }
}
