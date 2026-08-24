import { Component, OnInit, Input } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Subject } from "rxjs";
import { DeviceSensor } from "src/app/modules/SharedDeviceModule/models/DeviceSensor";
import { AquariumDeviceService } from "../../aquarium-device.service";
import { DeviceConfiguration } from "../../models/DeviceConfiguration";
import { DeviceInformation } from "../../models/DeviceInformation";
import { DeviceScheduledJob } from "../../models/DeviceScheduledJob";

@Component({
  selector: 'device-scheduled-job-list',
  templateUrl: './device-scheduled-job-list.component.html',
  styleUrls: []
})
export class DeviceScheduledJobListComponent implements OnInit {

  @Input() public deviceInformation: DeviceInformation;
  @Input() public scheduledJobs: DeviceScheduledJob[] = null;

  public loading: boolean = false;
  public error: string;
  public sensors: DeviceSensor[];
  public disabled: boolean = false;

  public readableTypes = new Subject<any>();

  constructor(private service: AquariumDeviceService,
    public dialog: MatDialog) { }

  ngOnInit() {
    if(this.scheduledJobs == null)
    {
      this.scheduledJobs = this.deviceInformation.scheduleStatus.scheduled;
    }
  }
}