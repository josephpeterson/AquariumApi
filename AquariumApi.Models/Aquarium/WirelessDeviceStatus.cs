using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class WirelessDeviceStatus
    {
        public string Hostname { get; set; }
        public string SessionId { get; set; }
        public long SessionStartTime { get; set; }
        public long SessionExpirationTime { get; set; }
        public List<WirelessDeviceSensor> Valves { get; set; }
        public string Actions { get; set; }
    }
}