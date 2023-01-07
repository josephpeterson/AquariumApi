using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class WirelessDevice
    {
        public int? Id { get; set; }
        public List<WirelessDeviceSensor> Sensors { get; set; }
        public string Name { get; set; }
        public string Hostname { get; set; }
        public DateTime LastConnected { get; set; }
        [NotMapped]
        public WirelessDeviceStatus Status { get; set; }
    }
}