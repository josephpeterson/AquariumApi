import { Component, OnInit, Input } from '@angular/core';
import { AquariumMixingStation } from 'src/app/modules/SharedDeviceModule/models/AquariumMixingStation';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { DeviceInformation } from '../../models/DeviceInformation';
import { AquariumMixingStationStatus } from '../../models/AquariumMixingStationStatus';
import { Store } from '@ngrx/store';
import { connectToDevice, deviceConnectionSuccess } from '../../store/device.actions';
@Component({
  selector: 'device-mixing-station-search',
  templateUrl: './device-mixing-station-search.component.html'
})
export class DeviceMixingStationSearchComponent {
  @Input() deviceInformation: DeviceInformation;
  public mixingStation: AquariumMixingStation;
  public searchingHostname = "mixingstation";
  public disabled = false;
  public searching = false;
  public connectingStation: AquariumMixingStationStatus = null;
  public mixingStationResults: AquariumMixingStationStatus[] = [];

  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }

  public clickSearchByHostname() {
    this.disabled = true;
    this.searching = true;
    this.service.searchForMixingStation().subscribe((data: AquariumMixingStationStatus[]) => {
      this.mixingStationResults = data;
      this.disabled = false;
      this.searching = false;
    }, () => {
      this.searching = false;
      this.disabled = false;
    });
  }
  public clickConnect(station: AquariumMixingStationStatus) {
    this.connectingStation = station;
    this.service.upsertMixingStation(station).subscribe((newConfiguration: DeviceConfiguration) => {
      console.log("Updated configuration", newConfiguration);
      this.store.dispatch(connectToDevice());
      this.connectingStation = null;
      this.notifier.success("Connected to mixing station");
    });
  }
}
