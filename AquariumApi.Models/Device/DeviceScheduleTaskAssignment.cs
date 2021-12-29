using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblDeviceScheduleTaskAssignment")]
    public class DeviceScheduleTaskAssignment
    {
        [Required]
        public int? Id { get; set; }
        public int ScheduleId { get; set; }
        public int TaskId { get; set; }
        public DateTime StartTime { get; set; }
        public TriggerTypes TriggerTypeId { get; set; }
        public int? TriggerTaskId { get; set; }
        public int? TriggerSensorId { get; set; }
        public GpioPinValue TriggerSensorValue { get; set; }
        public bool Repeat { get; set; }
        public int? RepeatInterval { get; set; }
        public DateTime RepeatEndTime { get; set; }
        public string DateConditions { get; set; }


        [ForeignKey("ScheduleId")]
        public DeviceSchedule Schedule { get; set; }
        [ForeignKey("TaskId")]
        public DeviceScheduleTask Task { get; set; }
        [ForeignKey("TriggerSensorId")]
        public DeviceSensor TriggerSensor { get; set; }
        [ForeignKey("TriggerTaskId")]
        public DeviceScheduleTask TriggerTask { get; set; }

        public bool RunsOnDate(DateTime date)
        {
            var day = date.DayOfWeek;
            if (string.IsNullOrEmpty(DateConditions))
                return true;
            if (DateConditions.Length < 7)
                return int.Parse(DateConditions) == date.Day;
            else
            {
                var dayCondition = DateConditions[(int)day];
                var runs = dayCondition.Equals('1');
                return runs;
            }
        }
    }
}
