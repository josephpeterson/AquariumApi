import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { WirelessDevice } from 'src/app/modules/SharedDeviceModule/models/WirelessDevice';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { DeviceInformation } from '../../models/DeviceInformation';
import { WirelessDeviceStatus } from '../../models/WirelessDeviceStatus';
import { Store } from '@ngrx/store';
import { connectToDevice, connectToMixingStation } from '../../store/device.actions';
import { DeviceSensor } from '../../models/DeviceSensor';
import { WirelessDeviceSensor } from '../../models/WirelessDeviceSensor';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { Observable } from 'rxjs';
@Component({
  selector: 'device-wireless-device-sensor-list',
  templateUrl: './device-wireless-device-sensor-list.component.html'
})
export class DeviceWirelessDeviceSensorListComponent {
  @Input() wirelessDevice: WirelessDevice;
  @Input() selectable: any = null;
  @Input() testable: any = null;
  @Input() disabled: any = null;
  @Input() inputModel: DeviceSensor;
  @Output() inputModelChange = new EventEmitter<DeviceSensor>();
  @Output() onChange = new EventEmitter();

  public configuredDevice$: Observable<DeviceConfiguration> = this.store.select(selectConfiguredDevice);

  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }

  public clickValve(sensor: WirelessDeviceSensor, configuredDevice: DeviceConfiguration) {
    if (this.testable != null)
      this.service.testMixingStationValve(sensor.id).subscribe(status => {
        this.store.dispatch(connectToMixingStation());
      })
    else if (this.selectable != null && !this.isUsed(sensor, configuredDevice)) {
      this.inputModel.pin = sensor.pin;
      this.inputModelChange.emit({
        ...this.inputModel,
        pin: sensor.pin
      });
      //this.onChange.emit(sensor.pin);
    }
  }
  public isSelected(sensor: WirelessDeviceSensor) {
    return sensor.pin == this.inputModel?.pin && this.wirelessDevice.id == this.inputModel?.locationId
  }
  public isUsed(sensor: WirelessDeviceSensor, configuredDevice: DeviceConfiguration) {
    var used = configuredDevice.sensors.filter(x =>
      x.locationId == this.wirelessDevice.id
      && x.pin == sensor.pin
      && x.id != this.inputModel?.id
    );
    if (used.length > 0)
      return used[0];
    return null;
  }
  public getTooltip(sensor: WirelessDeviceSensor, configuredDevice: DeviceConfiguration) {
    var str = "Sensor: " + sensor.id;
    if (sensor.value)
      str += "\nHigh";
    if (this.testable)
      str += '\nTest sensor';
    var used = this.isUsed(sensor, configuredDevice);
    if (used !== null)
      str += "\nAssigned to: " + used.name;
    return str;
  }
}
