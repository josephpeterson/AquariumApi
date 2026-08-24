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
  @Input() inputModel: number;
  @Output() inputModelChange = new EventEmitter<GpioPinTypes>();
  @Output() onChange = new EventEmitter();
  public boardModel;

  public gpioPorts: Array<Array<GpioPortStatus>> = [];

  public loading: boolean = false;
  public error: string;

  constructor(private aquariumDeviceService: AquariumDeviceService) {
  }
  ngOnInit() {
    this.boardModel = RaspberryPiModels.filter(x => x.name.indexOf(this.configuredDevice.settings.boardType) != -1)[0];
    if (this.boardModel)
      for (var i = 0; i < this.boardModel.gpioConfiguration.length; i++) {
        this.gpioPorts.push(this.boardModel.gpioConfiguration[i].map(gpio => {
          var boardPin = new GpioPortStatus();
          boardPin.type = gpio;
          boardPin.logicalBoardPin = this.boardModel.getLogicalPinLocation(gpio);
          boardPin.disabled = this.isPortDisabled(gpio);
          boardPin.selected = this.inputModel == boardPin.logicalBoardPin;
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
    return RaspberryPiUtils.getGpioConfiguration(this.configuredDevice.settings.boardType) != null;
  }
  isPortDisabled(portType: GpioPinTypes) {
    return portType == GpioPinTypes.pwr3V || portType == GpioPinTypes.pwr5V || portType == GpioPinTypes.Ground
  }
  isPortUsed(portType: GpioPinTypes): DeviceSensor {
    //check if port is in use
    var used;
    if (this.configuredDevice.sensors)
      this.configuredDevice.sensors.filter(x => x.locationId == null).forEach(s => {
        if (s.pin == this.boardModel.getLogicalPinLocation(portType) && s.pin != this.inputModel) {
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
    this.inputModelChange.emit(port.logicalBoardPin);
    this.onChange.emit(port.logicalBoardPin);
  }
}
export class GpioPortStatus {
  public type: GpioPinTypes;
  public logicalBoardPin: number;
  public selected: boolean = false;
  public disabled: boolean = false;
  public used: boolean = false;
  public sensor: DeviceSensor;
}