import { Component, OnInit, Input } from '@angular/core';

import { faChargingStation, faMinusCircle, faTrash } from '@fortawesome/free-solid-svg-icons';
import { DeviceScheduledJob } from 'src/app/models/DeviceScheduledJob';
import { AquariumDevice } from 'src/app/models/AquariumDevice';
import { JobStatus } from 'src/app/models/types/JobStatus';
import * as moment from 'moment';
import { GpioPinValue } from 'src/app/models/types/GpioPinValue';
import { ClientService } from '../services/client.service';
import { NotificationService } from '../services/notification.service';

@Component({
  selector: 'device-scheduled-job-list-item',
  templateUrl: './device-scheduled-job-list-item.component.html',
  styleUrls: ['./device-scheduled-job-list-item.component.scss']
})
export class DeviceScheduledJobListItemComponent implements OnInit {

  @Input("device") device: AquariumDevice;
  @Input("scheduledJob") scheduledJob: DeviceScheduledJob;

  public faMinusCircle = faMinusCircle;

  constructor(public _aquariumService: ClientService,
    public notifier: NotificationService) { }

  ngOnInit() {
  }
  public getSensorFromId(id:number) {
    var sensor = this.device.sensors.find(s => s.id == id);
    return sensor;
  }
  public getStatusFromJob(job:DeviceScheduledJob) {
    return JobStatus[job.status].toString();
  }
  public getSensorValueFromId(id:number) {
    return GpioPinValue[id].toString();
  }
  public isJobRunning(job:DeviceScheduledJob) {
    return job.status == JobStatus.Running;
  }
  public isJobCancelable(job:DeviceScheduledJob) {
    return job.status == JobStatus.Running;
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
  public clickStopScheduledJob(job:DeviceScheduledJob) {
    this._aquariumService.stopScheduledJob(job).subscribe(
      () => {
        this.notifier.notify("success", "Scheduled job stopped");
        //this.store.dispatch(new AquariumLoadDeployedDeviceByAquaruiumId(this.device.id));
      }, err => {
        this.notifier.notify("error", "Could not stop scheduled job");
      })
  }
  public getTaskFromJob(job: DeviceScheduledJob) {
    if(job.task) return job.task;
    var task = this.device.tasks.filter(t => t.id == job.taskId)[0];
    return task;
  }
  public readableDuration(time: string) {
    var d = moment(time).diff(moment());
    return moment.duration(d).humanize();
  }
  public readableDurationLength(startTime:string,endTime: string) {
    var d = moment(endTime).diff(moment(startTime));
    return moment.duration(d).humanize();
  }
  public readableTimestamp(time:string) {
    return moment(time).local().format("LT");
  }
  public readableDateTimestamp(time:string) {
    return moment(time).utc().local().calendar();
  }
}
