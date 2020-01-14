using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblDeviceScheduleAssignment")]
    public class DeviceScheduleAssignment
    {
        [Required]
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int ScheduleId { get; set; }
        public bool Deployed { get; set; }

        [ForeignKey("ScheduleId")]
        public DeviceSchedule Schedule { get; set; }

        [ForeignKey("DeviceId")]
        public AquariumDevice Device { get; set; }

    }
}
