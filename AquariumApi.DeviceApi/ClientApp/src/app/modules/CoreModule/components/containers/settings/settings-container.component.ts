import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { selectConfiguredDevice } from 'src/app/modules/SharedDeviceModule/store/device.selectors';
import { DeviceInformation } from '../../../../SharedDeviceModule/models/DeviceInformation';

@Component({
  selector: 'device-settings-container',
  templateUrl: './settings-container.component.html',
})
export class DeviceSettingsContainerComponent {
  public configuredDevice$ = this.store.select(selectConfiguredDevice);
  constructor(private store: Store) {
  }
}
