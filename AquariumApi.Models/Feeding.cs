using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblFeeding")]
    public class Feeding
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int AquariumId { get; set; }
        public int FishId { get; set; }
        public string FoodBrand { get; set; }
        public string FoodProduct { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }


        [ForeignKey("FishId")]
        public Fish Fish { get; set; }
        [ForeignKey("AquariumId")]
        public Aquarium Aquarium { get; set; }
    }
}