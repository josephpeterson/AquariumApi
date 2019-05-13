using AquariumApi.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.DataAccess
{
    [Table("tblWaterParameters")]
    public partial class tblWaterParameters
    {
        public int Id { get; set; }
        public double Ammonia { get; set; }
        public double Ph { get; set; }
        public int Temperature { get; set; }
        public double Nitrite { get; set; }
        public double Nitrate { get; set; }
        public int SnapshotId { get; set; }
        public AquariumSnapshot Snapshot { get; set; }
    }
}