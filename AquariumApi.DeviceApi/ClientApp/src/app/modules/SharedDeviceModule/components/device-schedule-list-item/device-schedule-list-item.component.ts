import { Component, Input, OnInit } from '@angular/core';
import { faEdit, faTrashAlt, faVial } from '@fortawesome/free-solid-svg-icons';
import { DeviceScheduleTask } from 'src/app/modules/SharedDeviceModule/models/DeviceScheduleTask';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { Store } from '@ngrx/store';
import { DeviceConfiguration } from '../../models/DeviceConfiguration';
import { DeviceSchedule } from '../../models/DeviceSchedule';
import * as moment from 'moment';

@Component({
  selector: 'device-schedule-list-item',
  templateUrl: './device-schedule-list-item.component.html',
  styleUrls: []
})
export class DeviceScheduleListItemComponent implements OnInit {
  public configuredDevice$ = this.store.select(selectConfiguredDevice);
  @Input() public hideEdit: any = null;
  @Input() public hideTest: any = null;
  @Input() schedule: DeviceSchedule;
  @Input() disabled: boolean;
  public faTrashAlt = faTrashAlt;
  public faEdit = faEdit;
  public faVial = faVial;


  constructor(public service: AquariumDeviceService, public store: Store) { }

  ngOnInit() {
  }
  public getSensorById(id: number, device: DeviceConfiguration) {
    return device.sensors.filter(x => x.id == id)[0];
  }
  public getTaskById(id: number, device: DeviceConfiguration) {
    return device.tasks.filter(x => x.id == id)[0];
  }
  public getReadableTime(date:string) {
    return moment(date).local().format("hh:mm a");
  }
  getDeviceTriggerTypes = () => this.service.getSelectOptionsByType("TriggerTypes");
}
