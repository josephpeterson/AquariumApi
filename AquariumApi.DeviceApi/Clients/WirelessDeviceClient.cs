using AquariumApi.Models.Constants;
using AquariumApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json.Serialization;

namespace AquariumApi.DeviceApi.Clients
{
    public class WirelessDeviceClient
    {
        private WirelessDevice _wirelessDevice;
        private JsonSerializerSettings jss = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.Indented
        };

        public WirelessDeviceClient(WirelessDevice wirelessDevice)
        {
            _wirelessDevice = wirelessDevice;
        }
        public WirelessDeviceClient(string wirelessDevice)
        {
            _wirelessDevice = new WirelessDevice()
            {
                Name = wirelessDevice,
                Hostname = wirelessDevice,
            };
        }
        public async Task<WirelessDeviceStatus> PingDevice()
        {
            try
            {
                var client = GetHttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var res = client.GetAsync(WirelessDeviceEndpoints.PING).Result;
                if (!res.IsSuccessStatusCode) throw new Exception("Invalid status code");

                var response = await res.Content.ReadAsStringAsync();
                WirelessDeviceStatus status = JsonConvert.DeserializeObject<WirelessDeviceStatus>(response);
                if (status.Hostname == null)
                    status.Hostname = _wirelessDevice.Hostname;
                return status;
            }
            catch (Exception e)
            {
                throw new DeviceException($"Could not find device by hostname '{_wirelessDevice.Hostname}'");
            }
        }
        public async Task<WirelessDeviceStatus> StopWirelessDevice()
        {
            var client = GetHttpClient();
            var res = await client.GetAsync(WirelessDeviceEndpoints.STOP);
            WirelessDeviceStatus status = JsonConvert.DeserializeObject<WirelessDeviceStatus>(res.Content.ReadAsStringAsync().Result);
            return status;
        }
        public async Task<WirelessDeviceStatus> TriggerWirelessDeviceSensor(int pin, GpioPinValue gpioPinValue)
        {
            var client = GetHttpClient();
            var url = WirelessDeviceEndpoints.TEST_VALVE.AggregateParams($"{pin}");
            var res = await client.GetAsync(url);
            if (!res.IsSuccessStatusCode)
                throw new Exception("Invalid response from wireless device.");
            var content = await res.Content.ReadAsStringAsync();
            WirelessDeviceStatus status = JsonConvert.DeserializeObject<WirelessDeviceStatus>(content);
            return status;
        }
        public async Task<WirelessDeviceStatus> TriggerWirelessDeviceSensor(List<WirelessDeviceSensor> sensorValues)
        {
            var client = GetHttpClient();
            var url = WirelessDeviceEndpoints.UPSERT_SESSION;
            var httpContent = new StringContent(JsonConvert.SerializeObject(sensorValues, jss), Encoding.UTF8, "application/json");
            var res = await client.PostAsync(url, httpContent);
            if (!res.IsSuccessStatusCode)
                throw new Exception("Invalid response from wireless device.");
            var content = await res.Content.ReadAsStringAsync();
            WirelessDeviceStatus status = JsonConvert.DeserializeObject<WirelessDeviceStatus>(content);
            return status;
        }
        private HttpClient GetHttpClient()
        {
            return new HttpClient()
            {
                BaseAddress = new Uri("http://" + _wirelessDevice.Hostname)
            };
        }
    }
}
