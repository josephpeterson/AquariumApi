using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        [ForeignKey("PhotoId")]
        public AquariumPhoto Photo { get; set; }
        public int? PhotoId { get; set; }
        [JsonProperty]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime Date { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Ammonia { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Nitrite { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Nitrate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Ph { get; set; }
        public int? Temperature { get; set; }

        public bool? ManualEntry { get; set; } = false;


    }
}
