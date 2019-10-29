using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblAquariumPhoto")]
    public class AquariumPhoto
    {
        [Required]
        public int? Id { get; set; }
        public int AquariumId { get; set; }
        [ForeignKey("AquariumId")]
        public Aquarium Aquarium { get; set; }
        public DateTime Date { get; set; }
        public string Filepath { get; set; }
        public AquariumSnapshot Snapshot { get; set; }
    }
}
