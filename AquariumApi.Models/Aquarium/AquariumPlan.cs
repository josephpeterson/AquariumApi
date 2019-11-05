using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblAquariumPlan")]
    public class AquariumPlan
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public bool Fish { get; set; }

        public bool Planted { get; set; }
        public bool LargeFish { get; set; }
        public bool PressurizedCo2 { get; set; }
        public bool HighMaintence { get; set; }
        public bool Bottomless { get; set; }
        public bool Breeder { get; set; }

        public virtual Aquarium Aquarium { get; set; }
    }
}
