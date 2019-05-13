using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblSnapshot")]
    public class AquariumSnapshot
    {
        [Required]
        public int Id { get; set; }
        [ForeignKey("AquariumId")]
        public Aquarium Aquarium { get; set; }
        public int AquariumId { get; set; }
        public DateTime Date { get; set; }
        public decimal Ammonia { get; set; }
        public decimal Nitrite { get; set; }
        public decimal Nitrate { get; set; }
        public decimal Ph { get; set; }
        public int Temperature { get; set; }

    }
}
