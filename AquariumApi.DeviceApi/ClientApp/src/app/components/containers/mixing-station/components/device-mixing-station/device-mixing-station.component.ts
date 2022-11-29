import { Component, OnInit, Input } from '@angular/core';
import { ClientService } from '../../../../../services/client.service';
import { NotifierService } from 'angular-notifier';
import { DeviceInformation } from '../../../../../models/DeviceInformation';
import { AquariumMixingStation } from 'src/app/models/AquariumMixingStation';
import { DeviceConfiguration } from 'src/app/models/DeviceConfiguration';

@Component({
  selector: 'device-mixing-station',
  templateUrl: './device-mixing-station.component.html',
  styleUrls: ['./device-mixing-station.component.scss']
})
export class DeviceMixingStationComponent implements OnInit {
  @Input() mixingStation: AquariumMixingStation;

  constructor(public service: ClientService,
    private notifier: NotifierService) { }

  ngOnInit() {
  }
  public clickDisconnect() {
    this.service.disconnectMixingStation().subscribe((newConfiguration: DeviceConfiguration) => {
      this.service.deviceInformation.configuredDevice = newConfiguration;
      window.location.reload();
    })
  }
}
