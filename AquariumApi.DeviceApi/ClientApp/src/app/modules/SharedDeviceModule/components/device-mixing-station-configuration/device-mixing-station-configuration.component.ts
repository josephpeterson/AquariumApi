import { Component, Input, OnInit } from '@angular/core';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { AquariumMixingStation } from 'src/app/modules/SharedDeviceModule/models/AquariumMixingStation';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { AquariumMixingStationStatus } from '../../models/AquariumMixingStationStatus';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice, selectMixingStationStatus } from '../../store/device.selectors';
import { connectToDevice, connectToMixingStation } from '../../store/device.actions';
@Component({
  selector: 'device-mixing-station-configuration',
  templateUrl: './device-mixing-station-configuration.component.html',
})
export class DeviceMixingStationConfigurationComponent implements OnInit {
  @Input() mixingStation: AquariumMixingStation;
  @Input() public mixingStationStatus;
  public configuredDevice$ = this.store.select(selectConfiguredDevice);
  public mixingStationConnecting: number = 0;

  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }
  public ngOnInit(): void {
    
  }
  public clickPing() {
    this.store.dispatch(connectToMixingStation());
  }
}
