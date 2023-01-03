import { Component, OnInit, Input } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Subject } from "rxjs";
import { DeviceSensor } from "src/app/modules/SharedDeviceModule/models/DeviceSensor";
import { NotificationService } from "src/app/services/notification.service";
import { AquariumDeviceService } from "../../aquarium-device.service";
import { DeviceConfiguration } from "../../models/DeviceConfiguration";
import { DeviceInformation } from "../../models/DeviceInformation";

@Component({
  selector: 'device-scheduled-job-list',
  templateUrl: './device-scheduled-job-list.component.html',
  styleUrls: []
})
export class DeviceScheduledJobListComponent implements OnInit {

  @Input() public deviceInformation: DeviceInformation;

  public loading: boolean = false;
  public error: string;
  public sensors: DeviceSensor[];
  public disabled: boolean = false;

  public readableTypes = new Subject<any>();

  constructor(private service: AquariumDeviceService,
    public notifier: NotificationService,
    public dialog: MatDialog) { }

  ngOnInit() {
  }
  getSensorReadableType(types, type: string) {
    if (!types) return "------";
    for (var i = 0; i < types.length; i++) {
      var t = types[i];
      if (t.value == type)
        return t.key;
    }
    return "Unknown";
  }
}