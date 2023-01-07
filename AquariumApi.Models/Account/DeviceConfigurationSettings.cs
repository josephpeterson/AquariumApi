using System.Runtime.Serialization;

namespace AquariumApi.Models
{
    public class DeviceConfigurationSettings
    {
        public string BoardType { get; set; }
        public string DeviceName { get; set; }
        public int Port { get; set; } = 80;
        public string Hostname { get; set; }
        public int DeviceSensorTestMaximumRuntime { get; set; }
        public string AquariumServiceUrl { get; set; }
        public int AuthTokenRenewTime { get; set; }
    }
}
