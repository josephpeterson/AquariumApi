using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblSpecies")]
    public class Species
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //Aggregate fields
        [NotMapped]
        public int AquariumCount { get; set; }
        [NotMapped]
        public int FishCount { get; set; }
    }
}