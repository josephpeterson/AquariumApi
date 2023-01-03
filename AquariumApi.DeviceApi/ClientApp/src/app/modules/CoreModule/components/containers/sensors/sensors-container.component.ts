import { Component } from '@angular/core';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceInformation } from '../../../../SharedDeviceModule/models/DeviceInformation';
import { selectDeviceInformation } from 'src/app/modules/SharedDeviceModule/store/device.selectors';
import { Store } from '@ngrx/store';

@Component({
  selector: 'device-sensors-overview-card-container',
  templateUrl: './sensors-container.component.html',
})
export class SensorsContainerComponent {
  public deviceInformation$ = this.store.select(selectDeviceInformation);

  constructor(private service: AquariumDeviceService, private store: Store) {
  }
}
