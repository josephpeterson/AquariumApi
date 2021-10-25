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
        public Aquarium Aquarium { get; set; }
        public object config { get; set; }
        public List<DeviceSchedule> Schedules { get; set; }
        public List<DeviceSensor> Sensors { get; set; }
    }
}
