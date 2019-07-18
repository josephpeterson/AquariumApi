using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IScheduleService
    {
    }
    public class ScheduleService : IScheduleService
    {
        private IConfiguration _config;
        private ILogger<ScheduleService> _logger;
        private Aquarium _aquarium;

        public ScheduleService(IConfiguration config, ILogger<ScheduleService> logger,Aquarium aquarium)
        {
            _config = config;
            _logger = logger;

            var t = Task.Run(() =>
            {
                var ticks = 0;
                while(true)
                {
                    ticks++;
                    tellApiToTakeSnapshot();
                    Thread.Sleep(15 * 60000);
                }
            });
        }


        //Lazy way
        public void tellApiToTakeSnapshot()
        {
            _logger.LogWarning("Taking snapshot...");

            var path = $"http://ec2-18-220-143-66.us-east-2.compute.amazonaws.com:8080/v1/Snapshot/{_aquarium.Id}/Take";

            Console.WriteLine("Aquarium: " + _aquarium.Name);

            return;
            /*
            HttpClient client = new HttpClient();
            var data = client.GetStringAsync(path).Result;
            var snapshot = JsonConvert.DeserializeObject<AquariumSnapshot>(data);
            snapshot.AquariumId = device.AquariumId.Value;
            */
        }
    }
}
