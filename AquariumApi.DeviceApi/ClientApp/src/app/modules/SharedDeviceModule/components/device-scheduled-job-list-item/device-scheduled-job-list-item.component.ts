import { Component, Input, OnInit } from '@angular/core';
import { faEdit, faTrashAlt, faVial } from '@fortawesome/free-solid-svg-icons';
import { DeviceScheduleTask } from 'src/app/modules/SharedDeviceModule/models/DeviceScheduleTask';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { Store } from '@ngrx/store';
import { DeviceConfiguration } from '../../models/DeviceConfiguration';
import { DeviceScheduledJob } from '../../models/DeviceScheduledJob';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { deviceGetScheduleArchive, deviceGetScheduleArchiveSuccess } from '../../store/device.actions';

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


  constructor(public service: AquariumDeviceService, public store: Store, public notifier: ToastrService) { }

  ngOnInit() {
    this.task = this.configuredDevice.tasks.filter(x => x.id == this.scheduledJob.taskId)[0];
  }
  public getReadableTime(date: string) {
    return moment(date).local().calendar();
  }
  public getReadableDuration(date: string, date2: string) {
    return moment.utc(moment(date2).diff(moment(date), 'milliseconds')).format("HH:mm:ss");
  }
  public clickRemoveAction() {
    this.service.removeScheduledJobFromArchive(this.scheduledJob).subscribe(x => {
      this.notifier.success(`Scheduled job ID: ${this.scheduledJob.id} was removed from archive.`);
      this.store.dispatch(deviceGetScheduleArchiveSuccess({content: x}));
    }, err => {
      var error = 'Error occured while removing scheduled job.';
      if (err.error?.message)
        error = err.error.message;
      this.notifier.error(error);
    });
  }
}
