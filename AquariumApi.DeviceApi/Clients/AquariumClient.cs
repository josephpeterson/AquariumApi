using AquariumApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi.Clients
{
    public interface IAquariumClient
    {
        AquariumSnapshot SendAquariumSnapshot(AquariumSnapshot snapshot, AquariumPhoto photo);
        AquariumDevice GetDeviceInformation(AquariumDevice aquariumDevice);
    }
    public class AquariumClient: IAquariumClient
    {
        private ILogger<IAquariumClient> _logger;
        private IConfiguration _config;
        private IDeviceService _deviceService;

        public AquariumClient(ILogger<IAquariumClient> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public AquariumDevice GetDeviceInformation(AquariumDevice aquariumDevice)
        {
            var path = $"{_config["AquariumServiceUrl"]}/Device/Ping";
            _logger.LogInformation("Service Url: " + path);

            var httpContent = new StringContent(JsonConvert.SerializeObject(aquariumDevice), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(1);
            var result = client.PostAsync(path, httpContent).Result;
            return JsonConvert.DeserializeObject<AquariumDevice>(result.Content.ReadAsStringAsync().Result);
        }

        public AquariumSnapshot SendAquariumSnapshot(AquariumSnapshot snapshot, AquariumPhoto photo)
        {
            throw new NotImplementedException();
        }

        /*
        public void tellApiToTakeSnapshot()
        {
            _logger.LogWarning("Taking snapshot...");

            var path = $"{_config["AquariumServiceUrl"]}/v1/Snapshot/{_aquarium.Id}/Take";

            Console.WriteLine("Aquarium: " + _aquarium.Name);

            return;
            HttpClient client = new HttpClient();
            var data = client.GetStringAsync(path).Result;
            var snapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(data);
            snapshot.AquariumId = device.AquariumId.Value;
    }

    public void SendAquariumSnapshot()
    {
        var path = $"{_config["AquariumServiceUrl"]}/v1/Device/{_aquarium.Id}/Take";
        var data = new
        {
            PhotoData = _deviceService.TakePhoto(_deviceService.GetAquarium().CameraConfiguration),
            Snapshot = _deviceService.TakeSnapshot()
        };
        HttpClient client = new HttpClient();
        var httpContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));

        var result = client.PostAsync(path, httpContent).Result;
        if (result.IsSuccessStatusCode)
            return true;
        var snapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(data);
        snapshot.AquariumId = device.AquariumId.Value;
    }

    public AquariumDevice PingAquariumService()
    {
        var path = $"{_config["AquariumServiceUrl"]}/v1/Device/Ping";
        HttpClient client = new HttpClient();
        var result = client.GetAsync(path).Result;
        return JsonConvert.DeserializeObject<AquariumDevice>(result.Content.ReadAsStringAsync().Result);
    }

    */
    }
}
