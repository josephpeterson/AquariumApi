import { Component, OnInit, Input } from '@angular/core';
import { AquariumDeviceService } from '../../../../../../SharedDeviceModule/aquarium-device.service';
import { DeviceInformation } from '../../../../../../SharedDeviceModule/models/DeviceInformation';
import { DeviceScheduledJob } from '../../../../../../SharedDeviceModule/models/DeviceScheduledJob';
import { JobStatus } from '../../../../../../../models/types/JobStatus';
import { ToastrService } from 'ngx-toastr';
import { DeviceScheduleState } from 'src/app/modules/SharedDeviceModule/models/DeviceScheduleState';
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


  constructor(public service: AquariumDeviceService,
    private notifier: ToastrService) { }

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
  public clickPerformScheduledJob(job:DeviceScheduledJob) {
    this.service.performScheduleTask(job.task).subscribe(
      () => {
        this.notifier.success("Performed scheduled task");

      }, err => {
        this.scanning = false;
        this.notifier.error("Could not perform scheduled task");
      })
  }
  public clickStopScheduledJob(job:DeviceScheduledJob) {
    this.service.stopScheduledJob(job).subscribe(
      () => {
        this.notifier.success("Scheduled job stopped");

      }, err => {
        this.scanning = false;
        this.notifier.error("Could not stop scheduled job");
      })
  }
}
