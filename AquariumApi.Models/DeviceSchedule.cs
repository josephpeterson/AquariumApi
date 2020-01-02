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
        [Required]
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }

        [ForeignKey("AuthorId")]
        public AquariumUser Author { get; set; }

        [ForeignKey("ScheduleId")]
        public ICollection<DeviceScheduleTask> Tasks { get; set; }

        [ForeignKey("ScheduleId")]
        public ICollection<DeviceScheduleAssignment> ScheduleAssignments { get; set; }

        [NotMapped]
        public bool Running { get; set; }
    }
}
