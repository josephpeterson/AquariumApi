import { Component, OnInit, Input } from '@angular/core';
import { LoginInformationResponse } from '../models/LoginInformationResponse';
import { DeviceScheduleState } from '../models/DeviceScheduleState';
import { ClientService } from '../services/client.service';
import * as moment from 'moment';
import { DeviceScheduleTask, DeviceScheduleTaskTypes } from '../models/DeviceScheduleTask';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'schedule-information',
  templateUrl: './schedule-information.component.html',
  styleUrls: ['./schedule-information.component.scss']
})
export class ScheduleInformationComponent implements OnInit {

  @Input() loginInformation: LoginInformationResponse;
  scheduleState: DeviceScheduleState;
  public scanning: boolean;
  performingTask: boolean;

  constructor(public service: ClientService,
    private notifier: NotifierService) { }

  ngOnInit() {

    this.clickGetDeviceScheduleStatus();
  }


  clickGetDeviceScheduleStatus() {
    this.scanning = true;
    this.service.getDeviceScheduleInformation().subscribe(
      (scheduleState: DeviceScheduleState) => {
        console.log(scheduleState);
        this.scheduleState = scheduleState;
      }, err => {
        this.scanning = false;
        //this.notifier.notify("error", "Could not retrieve device information");
      })
  }
  clickPerformTask(task: DeviceScheduleTask) {
    this.performingTask = true;
    this.service.performScheduleTask(task).subscribe(
      (data) => {
        this.performingTask = false;
        this.notifier.notify("success", "Task was performed successfully!");
      }, err => {
        this.performingTask = false;
        console.log(err);
        this.notifier.notify("error", "Task: " + err.error);
      })
  }
  public readableDuration(task: DeviceScheduleTask) {
    var d = moment(task.startTime).diff(moment());
    console.log(d);
    return moment.duration(d).humanize();
  }
  public readableDate(date: string) {
    return moment(date).local().calendar();
  }
  public getTaskNameFromId(taskId: number) {
    var types = DeviceScheduleTaskTypes;
    for (var name in types) {
      if(DeviceScheduleTaskTypes[name] == `${taskId}`)
        return name;
    }
    return DeviceScheduleTaskTypes.Unknown;
  }
}
