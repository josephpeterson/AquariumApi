using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblFishSnapshot")]
    public class FishSnapshot
    {
        [Required]
        public int Id { get; set; }
        [ForeignKey("AquariumId")]
        public Aquarium Aquarium { get; set; }
        public int AquariumId { get; set; }
        [ForeignKey("AquariumSnapshotId")]
        public AquariumSnapshot AquariumSnapshot { get; set; }
        public int AquariumSnapshotId { get; set; }
        public int FishId { get; set; }
        [ForeignKey("FishId")]
        public Fish Fish { get; set; }
        public int PhotoId { get; set; }
        [ForeignKey("PhotoId")]
        public FishPhoto FishPhoto { get; set; }

        public DateTime Date { get; set; }
        public string HealthStatus { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Length { get; set; }

    }
}
