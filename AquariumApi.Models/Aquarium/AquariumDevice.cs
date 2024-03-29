﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Models
{
    [Table("tblDevice")]
    public class AquariumDevice
    {
        [Required]
        public int Id { get; set; }
        public int AquariumId { get; set; }
        

        public string Name { get; set; }
        public string Type { get; set; }
        public string Port { get; set; }
        public string Address { get; set; }
        public string PrivateKey { get; set; }
        public string HardwareModel { get; set; }
        public bool EnabledTemperature { get; set; }
        public bool EnabledPh { get; set; }
        public bool EnabledPhoto { get; set; }
        public bool EnabledNitrate { get; set; }
        public bool EnabledNitrite { get; set; }
        public bool EnabledLighting { get; set; }

        public CameraConfiguration CameraConfiguration { get; set; }

        [ForeignKey("DeviceId")]
        public ICollection<DeviceSensor> Sensors { get; set; }
        [ForeignKey("DeviceId")]
        public virtual ICollection<DeviceScheduleTask> Tasks { get; set; }
        [ForeignKey("DeviceId")]
        public virtual ICollection<DeviceSchedule> Schedules { get; set; }
    }
}
