import { Component, OnInit, Input } from '@angular/core';
import { Aquarium } from '../models/Aquarium';
import { LoginInformationResponse } from '../models/LoginInformationResponse';

@Component({
  selector: 'information-page',
  templateUrl: './information-page.component.html',
  styleUrls: ['./information-page.component.scss']
})
export class InformationPageComponent implements OnInit {

  @Input() loginInformation: LoginInformationResponse;

  constructor() { }

  ngOnInit() {
    console.log(this.loginInformation);
  }

}
