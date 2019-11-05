using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblAquariumEquipment")]
    public class Equipment
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string ProductBrand { get; set; }
        public string SubBrand { get; set; }

        public virtual Aquarium Aquarium { get; set; }
    }
}
