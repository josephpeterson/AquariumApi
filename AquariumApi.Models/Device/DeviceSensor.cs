﻿
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class DeviceSensor
    {
        public int? Id { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public SensorTypes Type { get; set; }
        public int? LocationId { get; set; }
        [NotMapped]
        public GpioPinValue? Value { get; set; }
        [NotMapped]
        public bool Open { get; set; }
        public int Pin { get; set; }
        public bool AlwaysOn { get; set; }
        public Polarity Polarity {get; set; }

        public delegate void SensorTriggered(object sender,int value);


        public SensorTriggered OnSensorTriggered;

    }
    public enum SensorTypes
    {
        Other = 0,
        Sensor = 100,
        MixingStation = 200
    }
    public enum Polarity
    {
        Input = 0,
        Output = 1
    }

}
