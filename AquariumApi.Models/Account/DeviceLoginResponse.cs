using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class DeviceLoginResponse
    {
        public AquariumUser Account { get; set; }
        public Aquarium Aquarium { get; set; }
        public string Token { get; set; }
        public int? AquariumId { get; set; }
    }
}
