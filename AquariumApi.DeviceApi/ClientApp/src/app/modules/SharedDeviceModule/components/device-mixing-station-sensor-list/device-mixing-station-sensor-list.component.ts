import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AquariumMixingStation } from 'src/app/modules/SharedDeviceModule/models/AquariumMixingStation';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { ToastrService } from 'ngx-toastr';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { DeviceInformation } from '../../models/DeviceInformation';
import { AquariumMixingStationStatus } from '../../models/AquariumMixingStationStatus';
import { Store } from '@ngrx/store';
import { connectToDevice, connectToMixingStation } from '../../store/device.actions';
import { AquariumMixingStationValve } from '../../models/AquariumMixingStationValve';
@Component({
  selector: 'device-mixing-station-sensor-list',
  templateUrl: './device-mixing-station-sensor-list.component.html'
})
export class DeviceMixingStationSensorListComponent {
  @Input() mixingStationStatus: AquariumMixingStationStatus;
  @Input() selectable: any = null;
  @Input() testable: any = null;
  @Input() disabled: any = null;
  @Input() inputModel: any;
  @Output() inputModelChange = new EventEmitter<any>();
  @Output() onChange = new EventEmitter();

  constructor(public service: AquariumDeviceService,
    private store: Store,
    private notifier: ToastrService) { }

  public clickValve(valve) {
    if (this.selectable != null) {
      this.inputModelChange.emit(valve.pin);
      this.onChange.emit(valve.pin);
    }
  }
  public isSelected(valve) { return valve.pin == this.inputModel }
}
