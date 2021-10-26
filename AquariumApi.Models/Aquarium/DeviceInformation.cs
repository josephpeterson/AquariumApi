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
        public object Config {get; set;}
        public AquariumUser Account { get; set; }
        public Aquarium Aquarium { get; set; }
        public List<DeviceSchedule> Schedules { get; set; }
        public List<DeviceSensor> Sensors { get; set; }
        public ATOStatus ATOStatus { get; set; }
    }
}
