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
  public clickSubmitStep2() {
    this.loading = true;
    delete this.error;

    this.loginRequest.aquariumId = this.aquariumForm.aquarium.id;
    console.log(this.loginRequest);
    this.service.attemptLogin(this.loginRequest).subscribe((account: AquariumAccount) => {
      console.log("success 2 ", account);
      this.aquariumUser = account;
      this.currentStep = 3;
      this.loadInformation();
    }, (err: HttpErrorResponse) => {
      this.error = "Could not log in";
      this.loading = false;
      console.error(err);
    });
  }


  public loadInformation() {
    this.loading = true;
    delete this.error;
    this.service.getDetailedInformation().subscribe((data: LoginInformationResponse) => {
      // stuff
      console.log(data);
      this.loading = false;
      this.loginInformation = data;
    }, err => {
      this.error = "Could not authenticate with service"
      this.loading = false;
      this.currentStep = 2;
      console.error(err);
    });
  }
  public clickLogout() {
    this.service.logout();
  }
}

