using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;

public interface IGpioControllerWrapper
{
    void RegisterCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback);
    void OpenPin(int pinNumber, PinMode mode);
    bool IsPinOpen(int pinNumber);
    void Write(int pinNumber, PinValue value);
    PinValue Read(int pinNumber);
    void ClosePin(int pinNumber);

}
public class GpioControllerWrapper : IGpioControllerWrapper
{
    public GpioController Controller;
    public GpioControllerWrapper()
    {
        Controller = new GpioController();
    }
    public void ClosePin(int pinNumber) => Controller.ClosePin(pinNumber);

    public bool IsPinOpen(int pinNumber) => Controller.IsPinOpen(pinNumber);

    public void OpenPin(int pinNumber, PinMode mode) => Controller.OpenPin(pinNumber, mode);

    public void RegisterCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback) =>
        Controller.RegisterCallbackForPinValueChangedEvent(pinNumber, eventTypes, callback);


    public void Write(int pinNumber, PinValue value) => Controller.Write(pinNumber, value);
    public PinValue Read(int pinNumber) => Controller.Read(pinNumber);
}
public class MockGpioControllerWrapper : IGpioControllerWrapper
{
    Dictionary<int, PinValue> PinValues = new Dictionary<int, PinValue>();
    Dictionary<int, PinMode> PinModes = new Dictionary<int, PinMode>();

    public void ClosePin(int pinNumber)
    {
        int? pin = PinModes.Keys.Where(p => p == pinNumber).FirstOrDefault();
        if (pin != null)
        {
            PinModes.Remove(pin.Value);
            PinValues.Remove(pin.Value);
        }
    }

    public bool IsPinOpen(int pinNumber)
    {
        int? pin = PinModes.Keys.Where(p => p == pinNumber).FirstOrDefault();
        if (pin != null)
            return true;
        return false;
    }

    public void OpenPin(int pinNumber, PinMode mode)
    {
        ClosePin(pinNumber);
        PinModes.Add(pinNumber, mode);
    }

    public void RegisterCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback)
    {
       // throw new NotImplementedException();
    }

    public void Write(int pinNumber, PinValue value)
    {
        int? pin = PinModes.Keys.Where(p => p == pinNumber).FirstOrDefault();
        if(pin != null)
            PinValues.Remove(pin.Value);
        PinValues[pinNumber] = value;
    }
    public PinValue Read(int pinNumber)
    {
        var k = PinValues.Keys.Where(p => p == pinNumber).FirstOrDefault();
        var val = PinValues[k];
        return val;
    }
}