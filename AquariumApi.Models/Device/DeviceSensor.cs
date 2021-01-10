using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AquariumApi.Models
{
    public class DeviceSensor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SensorTypes Type { get; set; }
        public string Value { get; set; }
        public int Pin { get; set; }
        public Polarity Polarity {get; set; }

    }
    public enum SensorTypes
    {
        FloatSwitch,
        WaterLevel,
        Solenoid
    }
    public enum Polarity
    {
        Input = 0,
        Output = 1
    }
}
