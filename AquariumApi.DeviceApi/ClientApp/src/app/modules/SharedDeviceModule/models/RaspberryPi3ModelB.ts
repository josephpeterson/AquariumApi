import { GpioPinTypes } from './GpioPinTypes';

/* Raspberry pi models here */
export class RaspberryPi3ModelB {
  public name = "RaspberryPi 3 Model B";
  //public src: "./img/rpi3mb.png";
  public gpioConfiguration = [
    [
      GpioPinTypes.pwr3V,
      GpioPinTypes.gpio2,
      GpioPinTypes.gpio3,
      GpioPinTypes.gpio4,
      GpioPinTypes.Ground,
      GpioPinTypes.gpio17,
      GpioPinTypes.gpio27,
      GpioPinTypes.gpio22,
      GpioPinTypes.pwr3V,
      GpioPinTypes.gpio10,
      GpioPinTypes.gpio9,
      GpioPinTypes.gpio11,
      GpioPinTypes.Ground,
      GpioPinTypes.gpio0,
      GpioPinTypes.gpio5,
      GpioPinTypes.gpio6,
      GpioPinTypes.gpio13,
      GpioPinTypes.gpio19,
      GpioPinTypes.gpio26,
      GpioPinTypes.Ground,
    ],
    [
      GpioPinTypes.pwr5V,
      GpioPinTypes.pwr5V,
      GpioPinTypes.Ground,
      GpioPinTypes.gpio14,
      GpioPinTypes.gpio15,
      GpioPinTypes.gpio18,
      GpioPinTypes.Ground,
      GpioPinTypes.gpio23,
      GpioPinTypes.gpio24,
      GpioPinTypes.Ground,
      GpioPinTypes.gpio25,
      GpioPinTypes.gpio8,
      GpioPinTypes.gpio7,
      GpioPinTypes.gpio1,
      GpioPinTypes.Ground,
      GpioPinTypes.gpio12,
      GpioPinTypes.Ground,
      GpioPinTypes.gpio16,
      GpioPinTypes.gpio20,
      GpioPinTypes.gpio21
    ]
  ];
}
