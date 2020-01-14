using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblFishTreatment")]
    public class FishTreatment
    {
        [Required]
        [Key]
        public int Id { get; set; }
        //public int DiseaseId { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }
        public FishDisease Disease { get; set; }
    }
}