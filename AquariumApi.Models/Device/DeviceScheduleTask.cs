using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblDeviceScheduleTask")]
    public class DeviceScheduleTask
    {
        [Required]
        public int Id { get; set; }
        public ScheduleTaskTypes TaskId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Interval { get; set; }

        [ForeignKey("ScheduleId")]
        public DeviceSchedule Schedule { get; set; }

        public TimeSpan GetTaskETA()
        {
            return (StartTime - DateTime.UtcNow);
        }
    }   
}
