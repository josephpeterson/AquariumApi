import { Component, OnInit, ViewChild } from '@angular/core';
import { AquariumAccount } from 'src/app/models/AquariumAccount';
import { LoginInformationResponse } from 'src/app/models/LoginInformationResponse';
import { LoginInformationModel } from 'src/app/models/LoginInformation.model';
import { LoginFormComponent } from './login-form/login-form.component';
import { AquariumFormComponent } from './aquarium-form/aquarium-form.component';
import { HttpErrorResponse } from '@angular/common/http';
import { ClientService } from 'src/app/services/client.service';

@Component({
  selector: 'login-container',
  templateUrl: './login-container.component.html',
})
export class LoginContainerComponent implements OnInit {

  public error: string;
  public loading: boolean;
  public response: boolean;
  public currentStep: number = 1;
  public aquariumUser: AquariumAccount;
  public loginInformation: LoginInformationResponse;


  private loginRequest: LoginInformationModel = new LoginInformationModel();

  @ViewChild(LoginFormComponent) loginForm: LoginFormComponent
  @ViewChild(AquariumFormComponent) aquariumForm: AquariumFormComponent

  constructor(private service: ClientService) {

  }

  public ngOnInit(): void {

  }

  public clickSubmitStep1() {
    this.loading = true;
    delete this.error;

    this.loginRequest.email = this.loginForm.email;
    this.loginRequest.password = this.loginForm.password;
    this.service.attemptLogin(this.loginRequest).subscribe((account: AquariumAccount) => {
      console.log("success", account);
      this.aquariumUser = account;
      this.loading = false;
      this.currentStep = 2;
    }, (err: HttpErrorResponse) => {
      this.error = "Could not log in";
      this.loading = false;
      console.error(err);
    });
  }
  public clickSubmitStep2(force: boolean = false) {
    this.loading = true;
    delete this.error;

    if (this.currentStep == 2) {
      if (!this.aquariumForm.aquarium) {
        this.error = "Please select an aquarium";
        this.loading = false;
        return;
      }
      this.loginRequest.aquariumId = this.aquariumForm.aquarium.id;
      if (this.aquariumForm.aquarium.device && !force) {
        this.currentStep = 3;
        this.loading = false;
        return;
      }
    }

    var err = (err: HttpErrorResponse) => {
      this.error = "Could not log in";
      this.currentStep = 2;
      this.loading = false;
      console.error(err);
    };
    this.service.attemptLogin(this.loginRequest).subscribe((account: AquariumAccount) => {
      this.aquariumUser = account;
      this.currentStep = 4;

      this.service.getDeviceInformation().subscribe((data:LoginInformationResponse) => {
        this.service.loginInformation = data;
      },err);
    }, err);
  }
  public clickLogout() {
    this.service.logout();
  }
  public clickBack() {
    this.currentStep--;
  }
}
