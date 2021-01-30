using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class DeviceSensor
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public SensorTypes Type { get; set; }
        public string Value { get; set; }
        public int Pin { get; set; }
        public Polarity Polarity {get; set; }

        public delegate void SensorTriggered(object sender,int value);


        [JsonIgnore]
        public SensorTriggered OnSensorTriggered { get; set; }

        public virtual AquariumDevice Device { get; set; }


    }
    public enum SensorTypes
    {
        Other = 0,
        FloatSwitch = 100,
        ATOPumpRelay = 200,
        Solenoid = 300
    }
    public enum Polarity
    {
        Input = 0,
        Output = 1
    }

}
