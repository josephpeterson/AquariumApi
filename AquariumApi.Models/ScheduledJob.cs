using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AquariumApi.Models
{
    [Table("tblDeviceScheduleJobs")]
    public class ScheduledJob : Indexable  //allow pagination of this object
    {
        public int DeviceId { get; set; }
        public int TaskId { get; set; }
[JsonConverter(typeof(JsonStringEnumConverter))]
public JobStatus Status { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobEndReason EndReason { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? MaximumEndTime { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}