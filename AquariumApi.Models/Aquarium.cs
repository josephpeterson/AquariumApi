using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblAquarium")]
    public class Aquarium
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gallons { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }

    }
}