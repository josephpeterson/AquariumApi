using System;
using System.ComponentModel.DataAnnotations;

namespace AquariumApi.Models
{
    public class WaterDosing : Indexable
    {
        public int AquariumId { get; set; }
        public decimal Amount { get; set; }
        public string Brand { get; set; }
        public string Product { get; set; }
        public string Type { get; set; }

        public virtual Aquarium Aquarium { get; set; }
    }
}
