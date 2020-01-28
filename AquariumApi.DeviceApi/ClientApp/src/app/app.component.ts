import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { LoginFormComponent } from './login-form/login-form.component';
import { ClientService } from './services/client.service';
import { HttpErrorResponse } from '@angular/common/http';
import { Aquarium } from './models/Aquarium';
import { AquariumAccount } from './models/AquariumAccount';
import { LoginInformationModel } from './models/LoginInformation.model';
import { AquariumFormComponent } from './aquarium-form/aquarium-form.component';
import { AquariumDevice } from './models/AquariumDevice';
import { LoginInformationResponse } from './models/LoginInformationResponse';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Aquarium Device';

  public error: string;
  public loading: boolean;
  public response: boolean;
  public currentStep: number = 3;
  public aquariumUser: AquariumAccount;
  public loginInformation: LoginInformationResponse;


  private loginRequest: LoginInformationModel = new LoginInformationModel();

  @ViewChild(LoginFormComponent) loginForm: LoginFormComponent
  @ViewChild(AquariumFormComponent) aquariumForm: AquariumFormComponent

  constructor(private service: ClientService) {

  }

  public ngOnInit(): void {
    this.loadInformation();
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

    console.log(this.loginRequest);
    this.service.attemptLogin(this.loginRequest).subscribe((account: AquariumAccount) => {
      this.aquariumUser = account;
      this.currentStep = 4;
      this.loadInformation();
    }, (err: HttpErrorResponse) => {
      this.error = "Could not log in";
      this.currentStep = 2;
      this.loading = false;
      console.error(err);
    });
  }


  public loadInformation() {
    this.loading = true;
    delete this.error;
    this.service.getDetailedInformation().subscribe((data: LoginInformationResponse) => {
      // stuff
      this.loading = false;
      this.loginInformation = data;
      this.response = true;
      this.currentStep = 4;
    }, (err: HttpErrorResponse) => {
      this.response = true;
      console.log(err);
      if (err.status == 401) {
        this.currentStep = 1;
        this.loading = false;
        delete this.error;
        return;
      }
      this.error = "Could not authenticate with service";
      this.loading = false;
      this.currentStep = 1;
      console.error(err);
    });
  }
  public clickLogout() {
    this.service.logout();
  }
  public clickBack() {
    this.currentStep--;
  }
}

