import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';

import { GpioPinTypes } from '../../models/GpioPinTypes';
import { DeviceSensor } from 'src/app/modules/SharedDeviceModule/models/DeviceSensor';
import { RaspberryPiModels } from 'src/app/modules/SharedDeviceModule/models/RaspberyPiModels';
import { DeviceConfiguration } from '../../models/DeviceConfiguration';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { RaspberryPiUtils } from '../../models/RaspberryPiUtils';
import { DeviceSensorTypes } from '../../models/DeviceSensorTypes';

@Component({
  selector: 'device-sensor-gpio-board',
  templateUrl: './device-sensor-gpio-board.component.html',
  styleUrls: []
})
export class DeviceSensorGpioBoardComponent implements OnInit {
  @Input() public configuredDevice: DeviceConfiguration;
  @Input() public selectable: any = null;
  @Input() inputModel: GpioPinTypes;
  @Output() inputModelChange = new EventEmitter<GpioPinTypes>();
  @Output() onChange = new EventEmitter();

  public gpioPorts: Array<Array<GpioPortStatus>> = [];

  public loading: boolean = false;
  public error: string;

  constructor(private aquariumDeviceService: AquariumDeviceService) {
  }
  ngOnInit() {
    var boardModel = RaspberryPiModels.filter(x => x.name == this.configuredDevice.boardType)[0];
    for (var i = 0; i < boardModel.gpioConfiguration.length; i++) {
      this.gpioPorts.push(boardModel.gpioConfiguration[i].map(gpio => {
        var boardPin = new GpioPortStatus();
        boardPin.boardPinType = gpio;
        boardPin.disabled = this.isPortDisabled(gpio);
        boardPin.selected = this.inputModel == gpio;
        var used = this.isPortUsed(gpio);
        if (used) {
          boardPin.used = true;
          boardPin.sensor = used;
        }
        return boardPin;
      }));
    }
  }
  isBoardSupported() {
    return RaspberryPiUtils.getGpioConfiguration(this.configuredDevice.boardType) != null;
  }
  isPortDisabled(portType: GpioPinTypes) {
    return portType == GpioPinTypes.pwr3V || portType == GpioPinTypes.pwr5V || portType == GpioPinTypes.Ground
  }
  isPortUsed(portType: GpioPinTypes): DeviceSensor {
    //check if port is in use
    var used;
    if (this.configuredDevice.sensors)
      this.configuredDevice.sensors.filter(x => x.type == DeviceSensorTypes.Sensor).forEach(s => {
        if (s.pin == portType && s.pin != this.inputModel) {
          used = true;
          used = s;
        }
      });
    return used;
  }
  clickGpioPort(port: GpioPortStatus) {
    if (port.disabled || port.used || port.selected)
      return;
    if (this.selectable == null)
      return;
    this.gpioPorts.forEach(r => r.filter(x => x.selected).forEach(x => x.selected = false));
    port.selected = true;
    this.inputModelChange.emit(port.boardPinType);
    this.onChange.emit(port.boardPinType);
  }
}
export class GpioPortStatus {
  public boardPinType: GpioPinTypes;
  public selected: boolean = false;
  public disabled: boolean = false;
  public used: boolean = false;
  public sensor: DeviceSensor;
}