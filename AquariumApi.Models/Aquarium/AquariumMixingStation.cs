using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class AquariumMixingStation
    {
        public string SessionId { get; set; }
        public string Hostname { get; set; }
        public long SessionStartTime { get; set; }
        public long SessionExpirationTime { get; set; }
        public List<AquariumMixingStationValve> Valves { get; set; }
        public string Actions { get; set; }
    }
    public class AquariumMixingStationValve
    {
        public int Id { get; set; }
        public int Pin { get; set; }
        public bool Value { get; set; }
    }
}