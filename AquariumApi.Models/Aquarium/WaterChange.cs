using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class WaterChange : Indexable
    {
        public int AquariumId { get; set; }
        public int? ScheduleJobId { get; set; }
        public int GallonsAdded { get; set; }
        public int GallonsRemoved { get; set; }
        public DateTime EndTime { get; set; }

        public virtual Aquarium Aquarium { get; set; }
        public virtual ScheduledJob ScheduleJob { get; set; }
    }
}
