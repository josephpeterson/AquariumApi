import { Component, OnInit, Input } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Subject } from "rxjs";
import { DeviceSensor } from "src/app/modules/SharedDeviceModule/models/DeviceSensor";
import { NotificationService } from "src/app/services/notification.service";
import { AquariumDeviceService } from "../../aquarium-device.service";
import { DeviceConfiguration } from "../../models/DeviceConfiguration";

@Component({
  selector: 'device-sensor-list',
  templateUrl: './device-sensor-list.component.html',
  styleUrls: []
})
export class DeviceSensorListComponent implements OnInit {

  @Input() public configuredDevice: DeviceConfiguration;

  public loading: boolean = false;
  public error: string;
  public sensors: DeviceSensor[];
  public disabled: boolean = false;
  @Input() public hideEdit: any = null;
  @Input() public hideTest: any = null;
  @Input() public small: any = null;

  constructor(private service: AquariumDeviceService,
    public notifier: NotificationService,
    public dialog: MatDialog) { }

  ngOnInit() {
  }
}
