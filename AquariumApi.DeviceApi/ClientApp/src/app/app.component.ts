import { Component, OnInit } from '@angular/core';
import { AquariumDeviceService } from './modules/SharedDeviceModule/aquarium-device.service';
import { DeviceInformation } from './modules/SharedDeviceModule/models/DeviceInformation';
import { LoginInformationResponse } from './models/LoginInformationResponse';
import { DeviceConfiguration } from './modules/SharedDeviceModule/models/DeviceConfiguration';
import { Store } from '@ngrx/store';
import { selectDeviceConnection } from './modules/SharedDeviceModule/store/device.selectors';
import { connectToDevice, connectToMixingStation, loadDeviceTypesByType } from './modules/SharedDeviceModule/store/device.actions';
import { DeviceConnectionState } from './modules/SharedDeviceModule/store/device.reducer';
import { DeviceConnectionStatus } from './modules/SharedDeviceModule/models/RaspberyPiModels';

@Component({
  selector: 'device-app-root',
  templateUrl: './app.component.html',
  styleUrls: []
})
export class AppComponent implements OnInit {

  public loading = true;
  loadingAccount: boolean;
  //public deviceInformation: DeviceInformation;
  public deviceConnection$ = this.store.select(selectDeviceConnection);

  constructor(public service: AquariumDeviceService, private store: Store) {

  }

  public ngOnInit(): void {
    this.store.dispatch(connectToDevice());
    this.store.dispatch(connectToMixingStation());
    this.store.dispatch(loadDeviceTypesByType({ payload: "DeviceSensorTypes" }));
  }
  public isConnecting(state: DeviceConnectionState) {
    return state.deviceConnectionStatus == DeviceConnectionStatus.Connecting
  }
  public isConnected(state: DeviceConnectionState) {
    return state.deviceConnectionStatus == DeviceConnectionStatus.Connected || state.deviceConnection != null
  }
}

