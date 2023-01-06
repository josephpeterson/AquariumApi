import { Component, OnInit, Input } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { DeviceSensor } from "src/app/modules/SharedDeviceModule/models/DeviceSensor";
import { AquariumDeviceService } from "../../aquarium-device.service";
import { DeviceConfiguration } from "../../models/DeviceConfiguration";

@Component({
  selector: 'device-task-list',
  templateUrl: './device-task-list.component.html',
  styleUrls: []
})
export class DeviceTaskListComponent implements OnInit {

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