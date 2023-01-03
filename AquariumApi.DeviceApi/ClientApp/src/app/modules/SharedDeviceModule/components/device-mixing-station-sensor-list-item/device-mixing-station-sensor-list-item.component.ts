import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { AquariumMixingStation } from 'src/app/modules/SharedDeviceModule/models/AquariumMixingStation';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { DeviceInformation } from '../../models/DeviceInformation';
import { AquariumMixingStationStatus } from '../../models/AquariumMixingStationStatus';
import { Store } from '@ngrx/store';
import { connectToDevice, connectToMixingStation } from '../../store/device.actions';
import { AquariumMixingStationValve } from '../../models/AquariumMixingStationValve';
@Component({
  selector: 'device-mixing-station-sensor-list-item',
  templateUrl: './device-mixing-station-sensor-list-item.component.html',
  styleUrls: ['./device-mixing-station-sensor-list-item.component.scss']
})
export class DeviceMixingStationSensorListItemComponent {
  @Input() valve: AquariumMixingStationValve;
  @Input() selectable: any = null;
  @Input() testable: any = null;
  @Input() disabled: any = null;
  @Input() selected: any = null;
  @Output() onClick = new EventEmitter();

  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }

  public clickValve() {
    this.onClick.emit(this.valve);
    if (this.testable != null)
      this.service.testMixingStationValve(this.valve.id).subscribe(status => {
        this.store.dispatch(connectToMixingStation());
      })
  }
}
