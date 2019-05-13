using AquariumApi.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.DataAccess
{
    [Table("tblSnapshot")]
    public class tblSnapshot
    {
        [Required]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int WaterParameterId { get; set; }
        [ForeignKey("AquariumId")]
        public Aquarium Aquarium { get; set; }
        public int AquariumId { get; set; }
    }
}
