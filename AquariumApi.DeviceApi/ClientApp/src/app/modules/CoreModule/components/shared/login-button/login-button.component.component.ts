import { Component, OnInit, Input } from '@angular/core';
import { AquariumDeviceService } from '../../../../SharedDeviceModule/aquarium-device.service';
import { LoginInformationResponse } from '../../../../../models/LoginInformationResponse';
import { Aquarium } from 'src/app/models/Aquarium';
import { AquariumAccount } from 'src/app/modules/SharedDeviceModule/models/AquariumAccount';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { DeviceLoginModalComponent } from '../login-modal/login-modal.component';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice } from 'src/app/modules/SharedDeviceModule/store/device.selectors';
@Component({
  selector: 'device-login-button',
  templateUrl: './login-button.component.html',
  styleUrls: ['./login-button.component.scss']
})
export class DeviceLoginButton {

  public configuredDevice$ = this.store.select(selectConfiguredDevice);

  constructor(private store: Store,
    private dialog: MatDialog,
    private notifier: ToastrService) {
  }
  public clickLogin() {
    this.dialog.closeAll();
    this.dialog.open(DeviceLoginModalComponent, {
      width: '500px',
      disableClose: true
    });
  }
}
