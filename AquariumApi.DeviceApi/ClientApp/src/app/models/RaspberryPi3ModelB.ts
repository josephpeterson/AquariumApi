import { GpioPinTypes } from './GpioPinTypes';

/* Raspberry pi models here */
export class RaspberryPi3ModelB {
  public name: string = "RaspberryPi 3 Model B";
  //public src: "./img/rpi3mb.png";
  public gpioConfiguration =
    [GpioPinTypes.pwr3V,
    GpioPinTypes.pwr5V,
    GpioPinTypes.gpio2,
    GpioPinTypes.pwr5V,
    GpioPinTypes.gpio3,
    GpioPinTypes.Ground,
    GpioPinTypes.gpio4,
    GpioPinTypes.gpio14,
    GpioPinTypes.Ground,
    GpioPinTypes.gpio15,
    GpioPinTypes.gpio17,
    GpioPinTypes.gpio18,
    GpioPinTypes.gpio27,
    GpioPinTypes.Ground,
    GpioPinTypes.gpio22,
    GpioPinTypes.gpio23,
    GpioPinTypes.pwr3V,
    GpioPinTypes.gpio24,
    GpioPinTypes.gpio10,
    GpioPinTypes.Ground,
    GpioPinTypes.gpio9,
    GpioPinTypes.gpio25,
    GpioPinTypes.gpio11,
    GpioPinTypes.gpio8,
    GpioPinTypes.Ground,
    GpioPinTypes.gpio7,
    GpioPinTypes.gpio0,
    GpioPinTypes.gpio1,
    GpioPinTypes.gpio5,
    GpioPinTypes.Ground,
    GpioPinTypes.gpio6,
    GpioPinTypes.gpio12,
    GpioPinTypes.gpio13,
    GpioPinTypes.Ground,
    GpioPinTypes.gpio19,
    GpioPinTypes.gpio16,
    GpioPinTypes.gpio26,
    GpioPinTypes.gpio20,
    GpioPinTypes.Ground,
    GpioPinTypes.gpio21];
}
