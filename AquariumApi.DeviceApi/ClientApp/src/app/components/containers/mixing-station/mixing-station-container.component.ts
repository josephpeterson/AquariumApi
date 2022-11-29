import { Component, OnInit, ViewChild } from '@angular/core';
import { DeviceConfiguration } from 'src/app/models/DeviceConfiguration';
import { ClientService } from 'src/app/services/client.service';
import { DeviceInformation } from '../../../models/DeviceInformation';

@Component({
  selector: 'mixing-station-container',
  templateUrl: './mixing-station-container.component.html',
})
export class MixingStationContainer implements OnInit {
  public configuredDevice: DeviceConfiguration;

  constructor(private service: ClientService) {
    this.configuredDevice = this.service.deviceInformation.configuredDevice;
  }
  public ngOnInit(): void {

  }
}
