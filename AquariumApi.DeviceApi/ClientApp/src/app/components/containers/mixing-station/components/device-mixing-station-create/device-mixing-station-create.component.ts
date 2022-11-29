import { Component, OnInit, Input } from '@angular/core';
import { ClientService } from '../../../../../services/client.service';
import { NotifierService } from 'angular-notifier';
import { DeviceInformation } from '../../../../../models/DeviceInformation';
import { AquariumMixingStation } from 'src/app/models/AquariumMixingStation';
import { DeviceConfiguration } from 'src/app/models/DeviceConfiguration';

@Component({
  selector: 'device-mixing-station-create',
  templateUrl: './device-mixing-station-create.component.html',
  styleUrls: ['./device-mixing-station-create.component.scss']
})
export class DeviceMixingStationCreateComponent implements OnInit {
  @Input() deviceInformation: DeviceInformation;
  public mixingStation: AquariumMixingStation;
  public searchingHostname: string = "mixingstation";
  public disabled: boolean = false;
  public connectingStation: AquariumMixingStation = null;
  public mixingStationResults: AquariumMixingStation[] = [];

  constructor(public service: ClientService,
    private notifier: NotifierService) { }

  ngOnInit() {
  }
  public clickSearchByHostname() {
    console.log("Searching");
    this.disabled = true;
    this.service.searchMixingStationByHostname(this.searchingHostname).subscribe((data: AquariumMixingStation) => {
      this.mixingStationResults = [data];
      this.mixingStation = data;
      this.disabled = false;
    }, () => {
      this.disabled = false;
    });
  }
  public clickConnect(station: AquariumMixingStation) {
    this.connectingStation = station;
    this.service.upsertMixingStation(station).subscribe((newConfiguration: DeviceConfiguration) => {
      console.log("Updated configuration", newConfiguration);
      this.service.deviceInformation.configuredDevice = newConfiguration;
      this.connectingStation = null;
      this.notifier.notify("success", "Connected to mixing station");
    });
  }
}
