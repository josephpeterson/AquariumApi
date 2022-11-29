import { Component, OnInit, Input } from '@angular/core';
import { LoginInformationResponse } from '../../../../../models/LoginInformationResponse';
import { DeviceScheduleState } from '../../../../../models/DeviceScheduleState';
import { ClientService } from '../../../../../services/client.service';
import * as moment from 'moment';
import { NotifierService } from 'angular-notifier';
import { AquariumDevice } from '../../../../../models/AquariumDevice';
import { DeviceSensor } from '../../../../../models/DeviceSensor';
import { DetailedDeviceInformation } from '../../../../../models/DetailedDeviceInformation';
import { DeviceSensorTestRequest } from '../../../../../models/requests/DeviceSensorTestRequest';
import { HttpErrorResponse } from '@angular/common/http';
import { DeviceInformation } from '../../../../../models/DeviceInformation';
import { DeviceScheduledJob } from '../../../../../models/DeviceScheduledJob';
import { JobStatus } from '../../../../../models/types/JobStatus';

@Component({
  selector: 'device-job-list-item',
  templateUrl: './device-job-list-item.component.html',
  styleUrls: ['./device-job-list-item.component.scss']
})
export class DeviceJobListItemComponent implements OnInit {

  scheduleState: DeviceScheduleState;
  public scanning: boolean;
  @Input() public job: DeviceScheduledJob;
  @Input() public deviceInformation: DeviceInformation;

  JobStatus = JobStatus;


  constructor(public service: ClientService,
    private notifier: NotifierService) { }

  ngOnInit() {

    this.getDetailedDeviceInformation();
  }
  getDetailedDeviceInformation() {
    this.scanning = true;
    this.service.getDeviceInformation().subscribe(
      (deviceInfo: DeviceInformation) => {
        this.deviceInformation = deviceInfo;
      }, err => {
        this.scanning = false;
        //this.notifier.notify("error", "Could not retrieve device information");
      })
  }
  public getSensorFromJob(job:DeviceScheduledJob) {
    var sensor = this.deviceInformation.configuredDevice.sensors.find(s => s.id == job.task.targetSensorId);
    return sensor;
  }
  public clickPerformScheduledJob(job:DeviceScheduledJob) {
    this.service.performScheduleTask(job.task).subscribe(
      () => {
        this.notifier.notify("success", "Performed scheduled task");

      }, err => {
        this.scanning = false;
        this.notifier.notify("error", "Could not perform scheduled task");
      })
  }
  public clickStopScheduledJob(job:DeviceScheduledJob) {
    this.service.stopScheduledJob(job).subscribe(
      () => {
        this.notifier.notify("success", "Scheduled job stopped");

      }, err => {
        this.scanning = false;
        this.notifier.notify("error", "Could not stop scheduled job");
      })
  }
}
