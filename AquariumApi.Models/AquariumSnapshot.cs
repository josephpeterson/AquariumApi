using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class AquariumSnapshot
    {
        int Id { get; set; }
        Uri Src { get; set; }
        AquariumParameters Parameters { get; set; }
        DateTime Date { get; set; }

    }
}
