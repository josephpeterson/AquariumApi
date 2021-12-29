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
        public int DeviceId { get; set; }
        public ScheduleTaskTypes TaskTypeId { get; set; }
        public int TargetSensorId { get; set; }
        public GpioPinValue TargetSensorValue { get; set; }
        public int? TriggerSensorId { get; set; }
        public GpioPinValue TriggerSensorValue { get; set; }
        public int? MaximumRuntime { get; set; }

        [ForeignKey("DeviceId")]
        public virtual AquariumDevice Device { get; set; }
        [ForeignKey("TargetSensorId")]
        public virtual DeviceSensor TargetSensor { get; set; }
        [ForeignKey("TriggerSensorId")]
        public virtual DeviceSensor TriggerSensor { get; set; }
    }   
}
