using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class DeviceAction
    { 
        public int? Id { get; set; } = null;
        public int TargetSensorId { get; set; }
        public GpioPinValue TargetSensorValue { get; set; }

        [ForeignKey("TargetSensorId")]
        public virtual DeviceSensor TargetSensor { get; set; }
    }
}
