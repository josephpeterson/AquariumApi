using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class AquariumParameters
    {
        int Temperature { get; set; }
        float Ph { get; set; }
        float Nitrate { get; set; }
        float Nitrite { get; set; }
    }
}
