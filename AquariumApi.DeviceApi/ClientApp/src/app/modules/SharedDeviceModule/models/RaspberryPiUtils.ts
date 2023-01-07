import { RaspberryPiModels } from "./RaspberyPiModels";

export class RaspberryPiUtils {
  static getGpioConfiguration(model: string) {
    for (var i = 0; i < RaspberryPiModels.length; i++) {
      var m = RaspberryPiModels[i];
      if (m.name.indexOf(model) != -1)
        return m.gpioConfiguration;
    }
    return [];
  }
}
