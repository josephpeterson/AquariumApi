using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class WaterChange
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AquariumId { get; set; }
        public int GallonsAdded { get; set; }
        public int GallonsRemoved { get; set; }
        public DateTime Date { get; set; }

        public virtual Aquarium Aquarium { get; set; }
    }
}
