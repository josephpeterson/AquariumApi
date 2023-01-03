import { Component, Input } from '@angular/core';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { DeviceSchedule } from 'src/app/modules/SharedDeviceModule/models/DeviceSchedule';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { Store } from '@ngrx/store';
import { DeviceAction } from '../../models/DeviceAction';
import { DeviceSensor } from '../../models/DeviceSensor';
import { DeviceConfiguration } from '../../models/DeviceConfiguration';
import { GpioPinValue } from '../../models/GpioPinValue';
import { DeviceSensorPolarity } from '../../models/DeviceSensorPolarity';

@Component({
  selector: 'device-task-action-list-item',
  templateUrl: './device-task-action-list-item.component.html',
  styleUrls: []
})
export class DeviceTaskActionListItemComponent {
  public configuredDevice$ = this.store.select(selectConfiguredDevice);

  @Input() editable: string = null;
  @Input() action: DeviceAction;
  @Input() schedule: DeviceSchedule;
  @Input() disabled: boolean;
  public faTrashAlt = faTrashAlt;

  public targetSensor: DeviceSensor;


  constructor(public service: AquariumDeviceService, public store: Store) { }

  public getSensorById(id:number,deviceConfiguration:DeviceConfiguration)
  {
    return deviceConfiguration.sensors.filter(x => x.id == id)[0];
  }
  public getWriteableSensors(deviceConfiguration:DeviceConfiguration) {
    return deviceConfiguration.sensors.filter(s => s.polarity == DeviceSensorPolarity.Write)
  }
  getDeviceTriggerTypes = () => this.service.getSelectOptionsByType("TriggerTypes");
  getDeviceSensorValues = () => this.service.getSelectOptionsByType("DeviceSensorValues");
}
