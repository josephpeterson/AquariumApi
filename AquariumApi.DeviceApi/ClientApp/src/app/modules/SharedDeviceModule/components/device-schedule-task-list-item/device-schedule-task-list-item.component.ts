import { Component, Input, OnInit } from '@angular/core';
import { faEdit, faTrashAlt, faVial } from '@fortawesome/free-solid-svg-icons';
import { DeviceScheduleTask } from 'src/app/modules/SharedDeviceModule/models/DeviceScheduleTask';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { Store } from '@ngrx/store';
import { DeviceConfiguration } from '../../models/DeviceConfiguration';
import { connectToDevice } from '../../store/device.actions';

@Component({
  selector: 'device-schedule-task-list-item',
  templateUrl: './device-schedule-task-list-item.component.html',
  styleUrls: []
})
export class DeviceScheduleTaskListItemComponent implements OnInit {
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
  public clickRemoveTask(deviceConfiguration: DeviceConfiguration, task: DeviceScheduleTask) {
    this.service.deleteDeviceTask(task).subscribe((data) => {
      if (data) {
        this.store.dispatch(connectToDevice());
        //this.clickUndoChanges();
      }
      else
        console.warn("Could not delete task...");
    });
  }
  getDeviceTriggerTypes = () => this.service.getSelectOptionsByType("TriggerTypes");
}
