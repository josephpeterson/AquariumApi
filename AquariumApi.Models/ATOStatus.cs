using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class ATOStatus : Indexable  //allow pagination of this object
    {
        public int AquariumId { get; set; }
        public int? ScheduleJobId { get; set; }
        public double MlPerSec { get; set; }
        public DateTime EndTime { get; set; }

        public virtual Aquarium Aquarium { get; set; }
        public virtual ScheduledJob ScheduleJob { get; set; }
    }
}