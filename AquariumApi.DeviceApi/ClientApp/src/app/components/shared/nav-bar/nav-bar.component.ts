import { Component, OnInit, Input } from '@angular/core';
import { ClientService } from '../../../services/client.service';
import { LoginInformationResponse } from '../../../models/LoginInformationResponse';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {

  @Input() loginInformation: LoginInformationResponse;
  public renewingToken: boolean;

  constructor(private service: ClientService,
    private notifier: NotifierService) { }

  ngOnInit() {
  }
  public clickLogout() {
    this.service.logout();
  }
  public isLoggedIn() {
    return this.loginInformation && this.loginInformation.aquarium != null; //todo change this
  }
  public clickRenew() {
    this.renewingToken = true;
    this.service.renewAuthToken().subscribe(data => {
      this.notifier.notify("success", "Token renewed!");
      this.renewingToken = false;

    }, err => {
      this.notifier.notify("error", "Error renewing auth token");
      this.renewingToken = false;

    })
  }
}
