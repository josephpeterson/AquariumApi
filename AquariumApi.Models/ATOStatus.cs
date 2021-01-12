using System;

namespace AquariumApi.Models
{
    public class ATOStatus
    {
        public string SensorValue { get; set; }
        public bool PumpRunning { get; set; }
        public int MaxRuntime { get; set; }
        public int RuntimeRemaining { get; set; }
        public bool RunIndefinitely { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EstimatedEndTime { get; set; }
    }
}