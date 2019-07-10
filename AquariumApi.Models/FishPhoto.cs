using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblFishPhoto")]
    public class FishPhoto
    {
        [Required]
        public int Id { get; set; }
        public int AquariumId { get; set; }
        [ForeignKey("AquariumId")]
        public Aquarium Aquarium { get; set; }
        public int FishId { get; set; }
        [ForeignKey("FishId")]
        public Fish Fish { get; set; }
        public DateTime Date { get; set; }
        public string Filepath { get; set; }

    }
}
