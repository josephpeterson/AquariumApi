import { Component, OnInit } from '@angular/core';
import { ClientService } from './services/client.service';
import { HttpErrorResponse } from '@angular/common/http';
import { LoginInformationResponse } from './models/LoginInformationResponse';

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
    this.service.getDeviceInformation().subscribe((data: LoginInformationResponse) => {
      this.loading = false;
      this.service.loginInformation = data;
    }, () => {
      this.loading = false;
    });
  }
}

