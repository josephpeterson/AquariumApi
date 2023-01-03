import { Component, OnInit, Input } from '@angular/core';
import { AquariumDeviceService } from '../../../SharedDeviceModule/aquarium-device.service';
import { DeviceInformation } from '../../../SharedDeviceModule/models/DeviceInformation';
import { AquariumMixingStation } from 'src/app/modules/SharedDeviceModule/models/AquariumMixingStation';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { AquariumMixingStationStatus } from 'src/app/modules/SharedDeviceModule/models/AquariumMixingStationStatus';
@Component({
  selector: 'device-mixing-station-create',
  templateUrl: './device-mixing-station-create.component.html',
  styleUrls: ['./device-mixing-station-create.component.scss']
})
export class DeviceMixingStationCreateComponent {
  @Input() deviceInformation: DeviceInformation;
  public mixingStation: AquariumMixingStation;
  public searchingHostname = "mixingstation";
  public disabled = false;
  public searching = false;
  public connectingStation: AquariumMixingStationStatus = null;
  public mixingStationResults: AquariumMixingStationStatus[] = [];

  constructor(public service: AquariumDeviceService,
    private notifier: ToastrService) { }

}
