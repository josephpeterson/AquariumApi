import { Component, OnInit } from '@angular/core';
import { ClientService } from './services/client.service';
import { HttpErrorResponse } from '@angular/common/http';
import { LoginInformationResponse } from './models/LoginInformationResponse';
import { DeviceInformation } from './models/DeviceInformation';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  public loading: boolean = true;

  constructor(public service: ClientService) {

  }

  public ngOnInit(): void {
    this.loadInformation();
  }
  public loadInformation() {
    this.loading = true;
    this.service.getDeviceInformation().subscribe((data: DeviceInformation) => {
      console.log(data);
      this.loading = false;
      this.service.deviceInformation = data;
    }, () => {
      this.loading = false;
    });
  }
  public isDeviceAvailable() {
    return this.service.deviceInformation != null;
  }
  public isLoggedIn() {
    if(!this.service.deviceInformation)
      return false;
    return this.service.deviceInformation.account != null; //todo change this
  }
}

