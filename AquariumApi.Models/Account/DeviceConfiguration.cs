using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class DeviceConfiguration
    {
        public string BoardType { get; set; }
        public Aquarium Aquarium { get; set; }

        public AquariumMixingStation MixingStation { get; set; }
        public List<DeviceSchedule> Schedules { get; set; } = new List<DeviceSchedule>();           //Schedules assigned to device
        public List<DeviceSensor> Sensors { get; set; } = new List<DeviceSensor>();                 //Sensors created on the device
        public List<DeviceScheduleTask> Tasks { get; set; } = new List<DeviceScheduleTask>();       //Tasks created on device
    }
}
