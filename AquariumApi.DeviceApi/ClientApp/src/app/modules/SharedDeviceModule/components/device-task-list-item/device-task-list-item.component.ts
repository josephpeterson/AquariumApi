import { Component, Input, OnInit } from '@angular/core';
import { faEdit, faTrashAlt, faVial } from '@fortawesome/free-solid-svg-icons';
import { DeviceScheduleTask } from 'src/app/modules/SharedDeviceModule/models/DeviceScheduleTask';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { Store } from '@ngrx/store';
import { DeviceConfiguration } from '../../models/DeviceConfiguration';

@Component({
  selector: 'device-task-list-item',
  templateUrl: './device-task-list-item.component.html',
  styleUrls: []
})
export class DeviceTaskListItemComponent implements OnInit {
  public configuredDevice$ = this.store.select(selectConfiguredDevice);
  @Input() public hideEdit: any = null;
  @Input() public hideTest: any = null;
  @Input() task: DeviceScheduleTask;
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
  getDeviceTriggerTypes = () => this.service.getSelectOptionsByType("TriggerTypes");
}
