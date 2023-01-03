import { Component, Input } from '@angular/core';

import { faMinusCircle } from '@fortawesome/free-solid-svg-icons';
import { DeviceScheduledJob } from 'src/app/modules/SharedDeviceModule/models/DeviceScheduledJob';
import { AquariumDevice } from 'src/app/models/AquariumDevice';
import { JobStatus } from 'src/app/models/types/JobStatus';
import * as moment from 'moment';
import { GpioPinValue } from 'src/app/modules/SharedDeviceModule/models/GpioPinValue';
import { AquariumDeviceService } from '../../../../../../SharedDeviceModule/aquarium-device.service';
import { NotificationService } from '../../../../../../../services/notification.service';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';

@Component({
  selector: 'device-scheduled-job-list-item',
  templateUrl: './device-scheduled-job-list-item.component.html',
  styleUrls: ['./device-scheduled-job-list-item.component.scss']
})
export class DeviceScheduledJobListItemComponent {

  @Input() configuredDevice: DeviceConfiguration;
  @Input() scheduledJob: DeviceScheduledJob;

  public faMinusCircle = faMinusCircle;

  constructor(public _aquariumService: AquariumDeviceService,
    public notifier: NotificationService) { }

  public getSensorFromId(id:number) {
    const sensor = this.configuredDevice.sensors.find(s => s.id == id);
    return sensor;
  }
  public getStatusFromJob(job:DeviceScheduledJob) {
    return JobStatus[job.status].toString();
  }
  public getSensorValueFromId(id:number) {
    return GpioPinValue[id].toString();
  }
  public clickPerformScheduledJob(job:DeviceScheduledJob) {
    this._aquariumService.performScheduleTask(job.task).subscribe(
      (newJob: DeviceScheduledJob) => {
        //todo we would reload stuff
        this.notifier.notify("success", "Performed scheduled task");
        //this.store.dispatch(new AquariumLoadDeployedDeviceByAquaruiumId(this.device.id));
      }, err => {
        this.notifier.notify("error", "Could not perform scheduled task");
      })
  }
  public getTaskFromJob(job: DeviceScheduledJob) {
    if(job.task) return job.task;
    const task = this.configuredDevice.tasks.filter(t => t.id == job.taskId)[0];
    return task;
  }
  public readableDuration(time: string) {
    const d = moment(time).diff(moment());
    return moment.duration(d).humanize();
  }
  public readableDurationLength(startTime:string,endTime: string) {
    const d = moment(endTime).diff(moment(startTime));
    return moment.duration(d).humanize();
  }
  public readableTimestamp(time:string) {
    return moment(time).local().format("LT");
  }
  public readableDateTimestamp(time:string) {
    return moment(time).utc().local().calendar();
  }
}
