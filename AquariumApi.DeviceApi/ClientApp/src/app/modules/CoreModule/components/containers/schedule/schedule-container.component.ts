import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { deviceGetScheduleArchive } from 'src/app/modules/SharedDeviceModule/store/device.actions';
import { selectConfiguredDevice, selectDeviceInformation } from 'src/app/modules/SharedDeviceModule/store/device.selectors';

@Component({
  selector: 'device-schedule-container',
  templateUrl: './schedule-container.component.html',
})
export class ScheduleContainerComponent implements OnInit {
  public configuredDevice$ = this.store.select(selectConfiguredDevice);
  public deviceInformation$ = this.store.select(selectDeviceInformation);

  constructor(private service: AquariumDeviceService,private store: Store) {
  }
  ngOnInit(): void {
    this.store.dispatch(deviceGetScheduleArchive());
  }
}
