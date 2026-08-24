namespace AquariumApi.Models
{
    public class WirelessDeviceSensor
    {
        public int Id { get; set; }
        public int Pin { get; set; }
        public GpioPinValue InitialValue { get; set; }
        public double Length { get; set; }
        public GpioPinValue Value { get; set; }
    }
}