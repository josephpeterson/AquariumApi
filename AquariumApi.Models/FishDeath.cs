using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblFishBreeds")]
    public class FishBreeding
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int MotherId { get; set; }
        public int FatherId { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }


        public Fish Mother { get; set; }
        public Fish Father { get; set; }
        public ICollection<Fish> Fish { get; set; }
    }
}