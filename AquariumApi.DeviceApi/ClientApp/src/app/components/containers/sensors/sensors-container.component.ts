import { Component, OnInit, ViewChild } from '@angular/core';
import { ClientService } from 'src/app/services/client.service';
import { DeviceInformation } from '../../../models/DeviceInformation';

@Component({
  selector: 'sensors-container',
  templateUrl: './sensors-container.component.html',
})
export class SensorsContainer implements OnInit {
  public deviceInformation: DeviceInformation;

  constructor(private service: ClientService) {
    this.deviceInformation = this.service.deviceInformation;
  }
  public ngOnInit(): void {

  }
}
