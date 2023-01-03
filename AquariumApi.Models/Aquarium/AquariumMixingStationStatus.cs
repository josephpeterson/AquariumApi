using System;
using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class AquariumMixingStationStatus
    {
        public string Hostname { get; set; }
        public string SessionId { get; set; }
        public long SessionStartTime { get; set; }
        public long SessionExpirationTime { get; set; }
        public List<AquariumMixingStationValve> Valves { get; set; }
        public string Actions { get; set; }
    }
    public class AquariumMixingStationValve
    {
        public int Id { get; set; }
        public int Pin { get; set; }
        public GpioPinValue Value { get; set; }
    }
    public class AquariumMixingStation
    {
        public int SensorCount { get; set; }
        public string Hostname { get; set; }
        public DateTime LastConnected { get; set; }
    }
}