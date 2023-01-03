import { Component, Input, OnInit } from '@angular/core';
import { faEdit, faTrashAlt, faVial } from '@fortawesome/free-solid-svg-icons';
import { DeviceScheduleTask } from 'src/app/modules/SharedDeviceModule/models/DeviceScheduleTask';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { Store } from '@ngrx/store';
import { DeviceConfiguration } from '../../models/DeviceConfiguration';
import { DeviceScheduledJob } from '../../models/DeviceScheduledJob';
import * as moment from 'moment';

@Component({
  selector: 'device-scheduled-job-list-item',
  templateUrl: './device-scheduled-job-list-item.component.html',
  styleUrls: []
})
export class DeviceScheduledJobListItemComponent implements OnInit {
  @Input() public configuredDevice: DeviceConfiguration;
  @Input() public scheduledJob: DeviceScheduledJob;
  public task: DeviceScheduleTask;
  public faTrashAlt = faTrashAlt;
  public faEdit = faEdit;
  public faVial = faVial;


  constructor(public service: AquariumDeviceService, public store: Store) { }

  ngOnInit() {
    this.task = this.configuredDevice.tasks.filter(x => x.id == this.scheduledJob.taskId)[0];
  }
  public getReadableTime(date:string) {
    return moment(date).local().calendar();
  }
}
