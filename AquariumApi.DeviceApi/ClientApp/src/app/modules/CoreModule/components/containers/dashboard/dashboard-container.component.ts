import { Component, OnInit, ViewChild } from '@angular/core';
import { AquariumAccount } from 'src/app/modules/SharedDeviceModule/models/AquariumAccount';
import { LoginInformationResponse } from 'src/app/models/LoginInformationResponse';
import { LoginInformationModel } from 'src/app/modules/SharedDeviceModule/models/LoginInformation.model';
import { HttpErrorResponse } from '@angular/common/http';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceInformation } from 'src/app/modules/SharedDeviceModule/models/DeviceInformation';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { WirelessDeviceStatus } from 'src/app/modules/SharedDeviceModule/models/WirelessDeviceStatus';
import { Store } from '@ngrx/store';
import { selectDeviceAccount, selectDeviceInformation, selectMixingStationConnection } from 'src/app/modules/SharedDeviceModule/store/device.selectors';

@Component({
  selector: 'device-dashboard-container',
  templateUrl: './dashboard-container.component.html',
})
export class DashboardContainerComponent implements OnInit {

  public deviceInformation$ = this.store.select(selectDeviceInformation);
  public mixingStationStatus$ = this.store.select(selectMixingStationConnection);
  public account$ = this.store.select(selectDeviceAccount);

  constructor(private service: AquariumDeviceService,private store:Store) {
  }

  public ngOnInit(): void {
  }
}
