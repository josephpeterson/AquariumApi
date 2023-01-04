using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblDeviceSchedule")]
    public class DeviceSchedule
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int DeviceId { get; set; }
        public DateTime StartTime { get; set; }
        public List<DeviceScheduleTask> Tasks { get; set; }
        public bool Repeat { get; set; }
        public int RepeatInterval { get; set; }
        public DateTime RepeatEndTime { get; set; }
        public string DateConditions { get; set; }
    }
}
