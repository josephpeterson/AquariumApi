using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    [Table("tblFishDisease")]
    public class FishDisease
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public int DiseaseId { get; set; }
        public int FishId { get; set; }
        public int TreatmentId { get; set; }
        public int Level { get; set; }
        public bool Cured { get; set; }
        public string Notes { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }


        public Fish Fish { get; set; }
        public FishDisease Disease { get; set; }
        public FishTreatment Treatment { get; set; }
    }
}