import { Component, OnInit, Input } from '@angular/core';
import { LoginInformationResponse } from '../models/LoginInformationResponse';
import { AquariumDevice } from '../models/AquariumDevice';
import { ClientService } from '../services/client.service';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'hardware-information',
  templateUrl: './hardware-information.component.html',
  styleUrls: ['./hardware-information.component.scss']
})
export class HardwareInformationComponent implements OnInit {

  @Input() loginInformation: LoginInformationResponse;
  public device: AquariumDevice;
  public scanning: boolean;


  constructor(
    public service: ClientService,
    private notifier: NotifierService
  ){ }

  ngOnInit() {
    this.device = this.loginInformation.aquarium.device;
  }

  public clickScanDeviceHardware() {
    this.notifier.notify("warning","This is no longer implemented.")
  }

}
