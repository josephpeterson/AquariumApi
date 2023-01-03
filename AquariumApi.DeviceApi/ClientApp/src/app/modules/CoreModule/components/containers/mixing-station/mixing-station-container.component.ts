import { Component, OnInit, ViewChild } from '@angular/core';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceInformation } from '../../../../SharedDeviceModule/models/DeviceInformation';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice, selectMixingStationConnection, selectMixingStationStatus } from 'src/app/modules/SharedDeviceModule/store/device.selectors';
import { connectToMixingStation } from 'src/app/modules/SharedDeviceModule/store/device.actions';
import { DeviceConnectionStatus } from 'src/app/modules/SharedDeviceModule/models/RaspberyPiModels';
import { AquariumMixingStationStatus } from 'src/app/modules/SharedDeviceModule/models/AquariumMixingStationStatus';

@Component({
  selector: 'device-mixing-station-container',
  templateUrl: './mixing-station-container.component.html',
})
export class MixingStationContainerComponent implements OnInit {
  public configuredDevice$ = this.store.select(selectConfiguredDevice);
  public mixingStation$ = this.store.select(selectMixingStationConnection);
  public mixingStationConnectionStatus$ = this.store.select(selectMixingStationStatus);
  constructor(private store: Store) {
  }
  ngOnInit(): void {
    this.store.dispatch(connectToMixingStation());
  }

  public hasNoMixingStation(status: DeviceConnectionStatus, mixingStation: AquariumMixingStationStatus) {
    return status == DeviceConnectionStatus.Failed
  }
  public isLoading(status: DeviceConnectionStatus) {
    return status == DeviceConnectionStatus.Connecting
  }
}
