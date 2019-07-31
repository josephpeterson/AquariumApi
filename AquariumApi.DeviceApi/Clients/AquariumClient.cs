using AquariumApi.Models;
using Microsoft.AspNetCore.Mvc;
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
        AquariumSnapshot SendAquariumSnapshot(int deviceId,AquariumSnapshot snapshot, byte[] photo);
        AquariumDevice GetDeviceInformation(AquariumDevice aquariumDevice);
    }
    public class AquariumClient: IAquariumClient
    {
        private ILogger<IAquariumClient> _logger;
        private IConfiguration _config;
        private AquariumDevice _device;

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
            client.Timeout = TimeSpan.FromMinutes(5);
            var result = client.PostAsync(path, httpContent).Result;
            if (!result.IsSuccessStatusCode)
                throw new Exception("Service does not have this device key on record.");
            var device = JsonConvert.DeserializeObject<AquariumDevice>(result.Content.ReadAsStringAsync().Result);
            return device;
        }

        public AquariumSnapshot SendAquariumSnapshot(int deviceId,AquariumSnapshot snapshot, byte[] photo)
        {
            var path = $"{_config["AquariumServiceUrl"]}/Device/{deviceId}/Snapshot";
            _logger.LogInformation("Sending snapshot: " + path);


            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(new ByteArrayContent(photo), "snapshotImage", DateTime.Now.ToString());

            //var httpContent = new StringContent(JsonConvert.SerializeObject(snapshot), Encoding.UTF8, "application/json");
            multiContent.Add(new StringContent(JsonConvert.SerializeObject(snapshot)),"Snapshot");
            HttpClient client = new HttpClient();
            var result = client.PostAsync(path, multiContent).Result;
            if (!result.IsSuccessStatusCode)
                throw new Exception("Could not upload aquarium snapshot");
            var actualSnapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(result.Content.ReadAsStringAsync().Result);
            _logger.LogInformation($"Snapshot saved successfully (SnapshotId: {actualSnapshot.Id})");
            return actualSnapshot;
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
