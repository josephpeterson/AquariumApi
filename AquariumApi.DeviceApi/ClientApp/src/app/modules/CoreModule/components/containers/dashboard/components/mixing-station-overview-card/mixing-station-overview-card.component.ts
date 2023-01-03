import { Component, Input } from '@angular/core';
import { AquariumDeviceService } from '../../../../../../SharedDeviceModule/aquarium-device.service';
import { ToastrService } from 'ngx-toastr';
import { AquariumMixingStationStatus } from 'src/app/modules/SharedDeviceModule/models/AquariumMixingStationStatus';
@Component({
  selector: 'device-mixing-station-overview-card',
  templateUrl: './mixing-station-overview-card.component.html',
  styleUrls: []
})
export class DeviceMixingStationOverviewCard {

  @Input() public mixingStationStatus: AquariumMixingStationStatus;

  constructor(public service: AquariumDeviceService,
    private notifier: ToastrService) { }

}
