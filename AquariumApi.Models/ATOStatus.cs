using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class ATOStatus
    {
        public int? Id { get; set; }
        public int DeviceId { get; set; }
        public bool PumpRunning { get; set; }
        public double MlPerSec { get; set; }
        public GpioPinValue? FloatSensorValue { get; set; }
        public int MaxRuntime { get; set; }
        public int RuntimeRemaining { get; set; }
        public bool RunIndefinitely { get; set; }
        public bool Completed { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EstimatedEndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string EndReason { get; set; }

        public virtual AquariumDevice Device { get; set; }
        [NotMapped]
        public virtual bool Enabled { get; set; } = false;
        [NotMapped]
        public virtual DateTime? NextRunTime { get; set; }
        [NotMapped]
        public virtual DeviceSensor PumpRelaySensor { get; set; }
        [NotMapped]
        public virtual DeviceSensor FloatSensor { get; set; }
    }
}