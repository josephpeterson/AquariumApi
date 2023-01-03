import { Component, OnInit, Input } from '@angular/core';
import { AquariumDeviceService } from '../../../../SharedDeviceModule/aquarium-device.service';
import { LoginInformationResponse } from '../../../../../models/LoginInformationResponse';
import { Aquarium } from 'src/app/models/Aquarium';
import { AquariumAccount } from 'src/app/modules/SharedDeviceModule/models/AquariumAccount';
import { ToastrService } from 'ngx-toastr';
import { DeviceInformation } from 'src/app/modules/SharedDeviceModule/models/DeviceInformation';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice, selectDeviceAccount } from 'src/app/modules/SharedDeviceModule/store/device.selectors';
import { Observable } from 'rxjs';
import { connectToDevice } from 'src/app/modules/SharedDeviceModule/store/device.actions';
@Component({
  selector: 'device-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent {

  public renewingToken: boolean;
  @Input() public deviceInformation: DeviceInformation;
  public configuredDevice$ = this.store.select(selectConfiguredDevice);
  public account$ = this.store.select(selectDeviceAccount);
  public account: AquariumAccount;

  constructor(private service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) {
  }

  public clickLogout() {
    this.service.logout().subscribe(x => {
      this.store.dispatch(connectToDevice());
    });
  }
  public clickRenew() {
    this.renewingToken = true;
    this.service.renewAuthToken().subscribe(data => {
      this.notifier.success("Token renewed!");
      this.renewingToken = false;

    }, err => {
      this.notifier.error("Error renewing auth token");
      this.renewingToken = false;

    })
  }
}
