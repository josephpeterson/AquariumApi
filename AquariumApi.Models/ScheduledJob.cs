﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public enum JobStatus
    {
        Ready,
        Running,
        Completed,
        Canceled,
        Errored
    }
    public enum JobEndReason
    {
        Normally,
        MaximumRuntimeReached,
        Error,
        Canceled,
        ForceStop,
    }
    public class ScheduledJob : Indexable  //allow pagination of this object
    {
        public int? Id { get; set; }
        public int DeviceId { get; set; }
        public int TaskId { get; set; }
        public JobStatus Status { get; set; }
        public JobEndReason EndReason { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? MaximumEndTime { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual DeviceScheduleTask Task { get; set; }
        public virtual AquariumDevice Device { get; set; }
    }
}