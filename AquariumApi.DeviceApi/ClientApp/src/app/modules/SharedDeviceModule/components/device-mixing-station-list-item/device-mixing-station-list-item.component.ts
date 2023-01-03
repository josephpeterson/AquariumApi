import { Component, OnInit, Input } from '@angular/core';
import { AquariumMixingStation } from 'src/app/modules/SharedDeviceModule/models/AquariumMixingStation';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { DeviceInformation } from '../../models/DeviceInformation';
import { AquariumMixingStationStatus } from '../../models/AquariumMixingStationStatus';
import { Store } from '@ngrx/store';
import { connectToDevice, connectToMixingStation } from '../../store/device.actions';
@Component({
  selector: 'device-mixing-station-list-item',
  templateUrl: './device-mixing-station-list-item.component.html'
})
export class DeviceMixingStationListItemComponent {
  @Input() mixingStationStatus: AquariumMixingStationStatus;
  @Input() public hideConnect: any = null;
  public disabled = false;
  public searching = false;
  public connectingStation: AquariumMixingStation = null;
  public mixingStationResults: AquariumMixingStation[] = [];

  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }

  public clickConnect() {
    this.service.upsertMixingStation(this.mixingStationStatus).subscribe((newConfiguration: DeviceConfiguration) => {
      this.store.dispatch(connectToDevice());
      this.store.dispatch(connectToMixingStation());
    });
  }
}
