using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class CreateAquariumRequest
    {
        public string Resource { get; set; }
        public Species Species { get; set; }
    }
}
