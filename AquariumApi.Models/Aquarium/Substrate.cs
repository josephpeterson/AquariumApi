using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblAquariumSubstrate")]
    public class Substrate
    {
        [Required]
        [Key]
        public int? Id { get; set; }
        public string Type { get; set; }
        public string ProductBrand { get; set; }
        public string Color { get; set; }
        public decimal Height { get; set; }
        public bool Inert { get; set; }

        public virtual Aquarium Aquarium { get; set; }
    }
}
