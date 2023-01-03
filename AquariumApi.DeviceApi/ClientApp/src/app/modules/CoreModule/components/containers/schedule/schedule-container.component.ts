import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { selectConfiguredDevice } from 'src/app/modules/SharedDeviceModule/store/device.selectors';

@Component({
  selector: 'device-schedule-container',
  templateUrl: './schedule-container.component.html',
})
export class ScheduleContainerComponent {
  public configuredDevice$ = this.store.select(selectConfiguredDevice);

  constructor(private service: AquariumDeviceService,private store: Store) {
  }
}
