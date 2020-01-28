import { Component, OnInit, ViewChild } from '@angular/core';
import { AquariumAccount } from 'src/app/models/AquariumAccount';
import { LoginInformationResponse } from 'src/app/models/LoginInformationResponse';
import { LoginInformationModel } from 'src/app/models/LoginInformation.model';
import { HttpErrorResponse } from '@angular/common/http';
import { ClientService } from 'src/app/services/client.service';

@Component({
  selector: 'dashboard-container',
  templateUrl: './dashboard-container.component.html',
})
export class DashboardContainerComponent implements OnInit {

  public loginInformation: LoginInformationResponse;

  constructor(private service: ClientService) {
  }

  public ngOnInit(): void {
    this.loginInformation = this.service.loginInformation;
  }
}
