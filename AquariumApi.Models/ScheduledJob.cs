using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblDeviceScheduleJobs")]
    public class ScheduledJob : Indexable  //allow pagination of this object
    {
        public int DeviceId { get; set; }
        public int TaskId { get; set; }
        public JobStatus Status { get; set; }
        public JobEndReason EndReason { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? MaximumEndTime { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public virtual AquariumDevice Device { get; set; }
        [NotMapped]
        public virtual ScheduledJob PreviousJob { get; set; }
    }
}