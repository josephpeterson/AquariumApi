import { Component, Input } from '@angular/core';
import { AquariumDeviceService } from '../../../../../../SharedDeviceModule/aquarium-device.service';
import { ToastrService } from 'ngx-toastr';
import { Aquarium } from 'src/app/models/Aquarium';
import { AquariumAccount } from 'src/app/modules/SharedDeviceModule/models/AquariumAccount';
@Component({
  selector: 'device-aquarium-overview-card',
  templateUrl: './aquarium-overview-card.component.html',
  styleUrls: ['./aquarium-overview-card.component.scss']
})
export class DeviceAquariumOverviewCardComponent {

  @Input() public aquarium: Aquarium;
  @Input() public account: AquariumAccount;

  constructor(public service: AquariumDeviceService,
    private notifier: ToastrService) { }

}
