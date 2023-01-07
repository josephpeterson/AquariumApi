using System.Collections.Generic;

namespace AquariumApi.Models
{
    public class DeviceConfiguration
    {
        public Aquarium Aquarium { get; set; }

        public List<WirelessDevice> WirelessDevices { get; set; } = new List<WirelessDevice>();
        public List<DeviceSchedule> Schedules { get; set; } = new List<DeviceSchedule>();           //Schedules assigned to device
        public List<DeviceSensor> Sensors { get; set; } = new List<DeviceSensor>();                 //Sensors created on the device
        public List<DeviceScheduleTask> Tasks { get; set; } = new List<DeviceScheduleTask>();       //Tasks created on device
        public DeviceConfigurationSettings Settings { get; set; } = new DeviceConfigurationSettings();
        public DeviceConfigurationPrivateSettings PrivateSettings { get; set; } = new DeviceConfigurationPrivateSettings();
    }

}
