import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';

import { AquariumDeviceService } from '../../aquarium-device.service';
import { GpioPinTypes } from '../../models/GpioPinTypes';
import { GpioPortStatus } from '../device-sensor-gpio-board/device-sensor-gpio-board.component';

@Component({
  selector: 'device-sensor-gpio-board-pin',
  templateUrl: './device-sensor-gpio-board-pin.component.html',
  styleUrls: ['./device-sensor-gpio-board-pin.component.scss']
})
export class DeviceSensorGpioBoardPinComponent implements OnInit {
  @Input() public boardPin: GpioPortStatus;
  @Input() public matTooltipPosition: any;
  @Output() onClick: EventEmitter<GpioPortStatus> = new EventEmitter();
  public GpioPinTypes: typeof GpioPinTypes = GpioPinTypes;


  constructor(private aquariumDeviceService: AquariumDeviceService) {
  }
  ngOnInit() {
  }
  getGpioToolTip(pin: GpioPortStatus) {
    var str = "Gpio Pin: " + pin.boardPinType;
    if(pin.used)
      str += "\n" + pin.sensor.name;
    return str;
  }
  public clickAction() {
    this.onClick.emit(this.boardPin);
  }
}