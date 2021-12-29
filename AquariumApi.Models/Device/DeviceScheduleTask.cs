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
        public string DateConditions { get; set; }

        [ForeignKey("DeviceId")]
        public virtual AquariumDevice Device { get; set; }
        [ForeignKey("TargetSensorId")]
        public virtual DeviceSensor TargetSensor { get; set; }
        [ForeignKey("TriggerSensorId")]
        public virtual DeviceSensor TriggerSensor { get; set; }



        public bool RunsOnDate(DateTime date)
        {
            var day = date.DayOfWeek;
            if (string.IsNullOrEmpty(DateConditions))
                return true;
            if(DateConditions.Length < 7)
                return int.Parse(DateConditions) == date.Day;
            else
            {
                var dayCondition = DateConditions[(int) day];
                var runs = dayCondition.Equals('1');
                return runs;
            }
        }
    }   
}
