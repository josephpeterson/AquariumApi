import { Component, OnInit, Input } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Subject } from "rxjs";
import { DeviceSensor } from "src/app/modules/SharedDeviceModule/models/DeviceSensor";
import { AquariumDeviceService } from "../../aquarium-device.service";
import { DeviceConfiguration } from "../../models/DeviceConfiguration";

@Component({
  selector: 'device-schedule-list',
  templateUrl: './device-schedule-list.component.html',
  styleUrls: []
})
export class DeviceScheduleListComponent implements OnInit {

  @Input() public configuredDevice: DeviceConfiguration;

  public loading: boolean = false;
  public error: string;
  public sensors: DeviceSensor[];
  public disabled: boolean = false;

  constructor(private service: AquariumDeviceService,
    public dialog: MatDialog) { }

  ngOnInit() {
  }
}