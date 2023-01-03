using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblDeviceTask")]
    public class DeviceScheduleTask
    {
        public int? Id { get; set; } = null;
        public string Name { get; set; }
        public List<DeviceAction> Actions { get; set; } = new List<DeviceAction>();
        public int? TriggerSensorId { get; set; }
        public GpioPinValue? TriggerSensorValue { get; set; }
        public int MaximumRuntime { get; set; }

        [ForeignKey("TriggerSensorId")]
        public virtual DeviceSensor TriggerSensor { get; set; }
        public ScheduleTaskTypes TaskTypeId { get; set; }
    }
}
