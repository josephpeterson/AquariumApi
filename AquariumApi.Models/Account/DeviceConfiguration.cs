using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class DeviceConfiguration
    {
        public Aquarium Aquarium { get; set; }
        public AquariumMixingStation MixingStation { get; set; }
        public List<DeviceSchedule> Schedules { get; set; } = new List<DeviceSchedule>();
        public List<DeviceSensor> Sensors { get; set; } = new List<DeviceSensor>();
        public List<DeviceScheduleTask> Tasks { get; set; } = new List<DeviceScheduleTask>();
    }
}
