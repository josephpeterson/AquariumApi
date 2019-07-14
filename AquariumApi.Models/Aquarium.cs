﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblAquarium")]
    public class Aquarium
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gallons { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }


        [ForeignKey("CameraConfigurationId")]
        public CameraConfiguration CameraConfiguration { get; set; }
        public AquariumDevice Device { get; set; }

        public ICollection<Fish> Fish { get; set; }
        public ICollection<Feeding> Feedings { get; set; }
        public ICollection<AquariumSnapshot> Snapshots { get; set; }
    }
}