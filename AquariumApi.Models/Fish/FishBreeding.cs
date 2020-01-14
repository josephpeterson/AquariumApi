using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblFishDeath")]
    public class FishDeath
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int FishId { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }


        public Fish Fish { get; set; }
    }
}