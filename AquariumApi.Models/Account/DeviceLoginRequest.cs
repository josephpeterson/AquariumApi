using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class DeviceLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int? AquariumId { get; set; }
    }
}
