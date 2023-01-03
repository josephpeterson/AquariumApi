import { Component, ViewChild } from '@angular/core';
import { AquariumAccount } from 'src/app/modules/SharedDeviceModule/models/AquariumAccount';
import { LoginInformationResponse } from 'src/app/models/LoginInformationResponse';
import { LoginInformationModel } from 'src/app/modules/SharedDeviceModule/models/LoginInformation.model';
import { LoginFormComponent } from './login-form/login-form.component';
import { AquariumFormComponent } from './aquarium-form/aquarium-form.component';
import { HttpErrorResponse } from '@angular/common/http';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceInformation } from 'src/app/modules/SharedDeviceModule/models/DeviceInformation';

@Component({
  selector: 'device-unavailable-container',
  templateUrl: './device-unavailable-container.component.html',
})
export class DeviceUnavailableContainerComponent {

  constructor(private service: AquariumDeviceService) {

  }
}
