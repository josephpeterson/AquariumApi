import { Component, OnInit, Input } from '@angular/core';
import { ClientService } from '../../services/client.service';
import { LoginInformationResponse } from '../../models/LoginInformationResponse';

@Component({
  selector: 'nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss']
})
export class NavBarComponent implements OnInit {

  @Input() loginInformation: LoginInformationResponse;
  
  constructor(private service: ClientService) { }

  ngOnInit() {
  }
  public clickLogout() {
    this.service.logout();
  }
}
