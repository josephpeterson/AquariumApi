using System;

namespace AquariumApi.Models
{
    public class DeviceSensorTestRequest
    {
        public int SensorId { get; set; }
        public int Runtime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DeviceId { get; set; }
    }
}
