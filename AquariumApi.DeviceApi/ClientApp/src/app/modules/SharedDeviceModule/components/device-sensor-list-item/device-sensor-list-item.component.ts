import { Component, OnInit, Input } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { IconDefinition, faSync, faTrash, faEdit, faVial, faCarBattery, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import { Observable, Subject } from "rxjs";
import { take } from "rxjs/operators";
import { Aquarium } from "src/app/models/Aquarium";
import { AquariumDevice } from "src/app/models/AquariumDevice";
import { DeviceSensor } from "src/app/modules/SharedDeviceModule/models/DeviceSensor";
import { GpioPinValue } from "src/app/modules/SharedDeviceModule/models/GpioPinValue";
import { NotificationService } from "src/app/services/notification.service";
import { DeviceSensorUpsertModalComponent } from "../modals/device-sensor-upsert-modal/device-sensor-upsert-modal.component";
import { AquariumDeviceService } from "../../aquarium-device.service";
import { DeviceSensorPolarity } from "../../models/DeviceSensorPolarity";
import { selectDeviceSensorTypes } from "../../store/device.selectors";
import { Store } from "@ngrx/store";

@Component({
  selector: 'device-sensor-list-item',
  templateUrl: './device-sensor-list-item.component.html',
  //styleUrls: ['./device-sensor-list-item.component.scss']
})
export class DeviceSensorListItemComponent implements OnInit {

  @Input() public sensor: DeviceSensor;
  public types$ = this.store.select(selectDeviceSensorTypes);
  @Input() public hideEdit: any = null;
  @Input() public hideTest: any = null;
  @Input() public small: any = null;

  public loading: boolean = false;
  public error: string;
  public sensors: DeviceSensor[];
  public disabled: boolean = false;

  public faRefresh: IconDefinition = faSync;
  public faTrash: IconDefinition = faTrash;
  public faEdit: IconDefinition = faEdit;
  public faVial: IconDefinition = faVial;
  public faCarBattery: IconDefinition = faCarBattery;
  faCheck = faCheckCircle;



  constructor(private service: AquariumDeviceService,
    private store: Store,
    public notifier: NotificationService,
    public dialog: MatDialog) { }

  ngOnInit() {
  }
  getSensorReadableType(types, type: number) {
    if (!types) return "------";
    for (var i = 0; i < types.length; i++) {
      var t = types[i];
      if (t.value == type)
        return t.key;
    }
    return "Unknown";
  }
  getSensorValue(sensor: DeviceSensor) {
    if (GpioPinValue[sensor.value] != undefined)
      return GpioPinValue[sensor.value].toString();
    return null;
  }
  public DeviceSensorPolarity = DeviceSensorPolarity;
}
