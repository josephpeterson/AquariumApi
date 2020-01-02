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
        public Aquarium Aquarium;
        public object config;
        public List<DeviceSchedule> Schedules;
    }
}
