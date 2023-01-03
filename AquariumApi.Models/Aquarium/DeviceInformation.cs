using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    public class DeviceInformation
    {
        public ScheduleState ScheduleStatus;
        public string Version {get; set;}
        public object Config {get; set; }
        public AquariumUser Account { get; set; }
        public DeviceConfiguration ConfiguredDevice { get; set; }
    }
}
