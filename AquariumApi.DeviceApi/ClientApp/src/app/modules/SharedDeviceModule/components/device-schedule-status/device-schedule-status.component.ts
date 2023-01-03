import { Component, OnInit, Input } from '@angular/core';
import { AquariumDeviceService } from '../../aquarium-device.service';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { Store } from '@ngrx/store';
import { deviceGetScheduleStatus, deviceGetSensorValues } from '../../store/device.actions';
import { DeviceInformation } from '../../models/DeviceInformation';
import { DeviceScheduledJob } from '../../models/DeviceScheduledJob';
@Component({
  selector: 'device-schedule-status',
  templateUrl: './device-schedule-status.component.html',
  styleUrls: []
})
export class DeviceScheduleStatusComponent implements OnInit {

  @Input() public deviceInformation: DeviceInformation;

  public scanning: boolean;
  performingTask: boolean;
  private timeout;


  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }

  ngOnInit(): void {
    var endTime = moment().add(moment.duration(30,"seconds"));
    if (this.deviceInformation.scheduleStatus.runningJobs.length)
      endTime = moment(this.deviceInformation.scheduleStatus.runningJobs[0].scheduledJob.maximumEndTime);
    else if (this.deviceInformation.scheduleStatus.nextTasks.length)
      endTime = moment(this.deviceInformation.scheduleStatus.nextTasks[0].startTime);
    var timeout = moment.duration(endTime.diff(moment())).asMilliseconds()+500
    if(timeout < 5 * 1000)
      timeout = 5 * 1000;
    this.timeout = setInterval(() => {
      this.store.dispatch(deviceGetScheduleStatus());
      this.store.dispatch(deviceGetSensorValues());
    }, timeout);
  }
  ngOnDestroy(): void {
    clearInterval(this.timeout);
  }
  public readableDuration(scheduleJob: DeviceScheduledJob,time:string) {
    if (!scheduleJob)
      return "------";
    var d = moment(time).diff(moment());
    var duration = moment.duration(d);
    return duration.humanize();
  }
  public readableDate(date: string) {
    return moment(date).local().calendar();
  }
  public getTaskById(taskId: number) {
    return this.deviceInformation.configuredDevice.tasks.filter(x => x.id == taskId)[0];
  }
  public getScheduleFromId(scheduleId: number) {
    return this.deviceInformation.configuredDevice.schedules.filter(x => x.id == scheduleId)[0];
  }
  public getActionCount() {
    var tasks = this.deviceInformation.scheduleStatus.scheduled
      .map(t => this.deviceInformation.configuredDevice.tasks.filter(tt => tt.id == t.taskId)[0])
      .map(t => t.actions)
    return [].concat(...tasks).length;
  }
  public clickStopScheduledJob(job: DeviceScheduledJob) {
    this.service.stopScheduledJob(job).subscribe(
      () => {
        this.store.dispatch(deviceGetScheduleStatus());
        this.store.dispatch(deviceGetSensorValues());
      }, err => {
        this.notifier.error("Could not stop scheduled job");
      })
  }
}
