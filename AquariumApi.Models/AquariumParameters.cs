using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class AquariumParameters
    {
        public int Id { get; set; }
        public int Temperature { get; set; }
        public double Ph { get; set; }
        public double Nitrate { get; set; }
        public double Nitrite { get; set; }


        [ForeignKey("SnapshotRefId")]
        public AquariumSnapshot Snapshot { get; set; }

    }
}
