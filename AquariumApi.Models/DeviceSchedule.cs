using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblDeviceSchdules")]
    public class DeviceSchedule
    {
        [Required]
        public int Id { get; set; }
        public int DeviceId { get; set; }
        [ForeignKey("DeviceId")]
        public AquariumDevice Device { get; set; }
    }
}
