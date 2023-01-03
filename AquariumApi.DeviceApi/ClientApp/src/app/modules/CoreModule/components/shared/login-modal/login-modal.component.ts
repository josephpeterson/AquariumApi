import { Component, OnInit, Input } from '@angular/core';
import { AquariumDeviceService } from '../../../../SharedDeviceModule/aquarium-device.service';
import { LoginInformationResponse } from '../../../../../models/LoginInformationResponse';
import { Aquarium } from 'src/app/models/Aquarium';
import { AquariumAccount } from 'src/app/modules/SharedDeviceModule/models/AquariumAccount';
import { ToastrService } from 'ngx-toastr';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialogRef } from '@angular/material/dialog';
import { Store } from '@ngrx/store';
import { connectToDevice } from 'src/app/modules/SharedDeviceModule/store/device.actions';
@Component({
  selector: 'device-login-modal',
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.scss']
})
export class DeviceLoginModalComponent {

  public aquarium: Aquarium;
  public account: AquariumAccount;
  public loading: boolean = false;
  public error: string = null;
  public step: number = 1;
  public availableAquariums: Aquarium[];
  public selectedAquarium: Aquarium;

  public email: string;
  public password: string;
  @Input() disabled: boolean;

  constructor(private service: AquariumDeviceService,
    private store: Store,
    private _dialogRef: MatDialogRef<DeviceLoginModalComponent>,
    private notifier: ToastrService) {
  }
  public clickLogin() {
    if (this.step == 2)
      this.clickLoginStep2();
    else
      this.clickLoginStep1();
  }
  public clickLoginStep1() {
    this.loading = true;
    delete this.error;

    this.service.attemptLogin({
      email: this.email,
      password: this.password,
      aquariumId: null
    }).subscribe((account: AquariumAccount) => {
      this.account = account;
      console.log("Found valid aquarium account... ", account);
      this.availableAquariums = this.account.aquariums.sort();
      this.loading = false;
      this.step = 2;
    }, (err: HttpErrorResponse) => {
      this.error = "Invalid username/password combination.";
      this.loading = false;
      console.error(err);
    });
  }
  public clickLoginStep2() {
    this.loading = true;
    this.service.attemptLogin({
      email: this.email,
      password: this.password,
      aquariumId: this.selectedAquarium.id
    }).subscribe((account: AquariumAccount) => {
      this.account = account;
      console.log("Successfully logged into aquarium.", account);
      this._dialogRef.close(account);
      this.store.dispatch(connectToDevice());
    }, (err: HttpErrorResponse) => {
      this.error = "Unknown error occurred while selecting aquarium.";
      this.loading = false;
      console.error(err);
    });
  }
  public clickSelectAquarium(aquarium: Aquarium) {
    this.selectedAquarium = aquarium;
  }
}