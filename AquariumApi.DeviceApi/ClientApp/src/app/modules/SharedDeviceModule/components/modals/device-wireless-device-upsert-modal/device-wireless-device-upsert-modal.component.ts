import { Component, OnInit, Input, Inject } from '@angular/core';
import { DeviceScheduleTask } from 'src/app/modules/SharedDeviceModule/models/DeviceScheduleTask';
import { DeviceSchedule } from 'src/app/modules/SharedDeviceModule/models/DeviceSchedule';

import { AquariumDevice } from 'src/app/models/AquariumDevice';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import * as moment from 'moment';
import { DeviceSensor } from 'src/app/modules/SharedDeviceModule/models/DeviceSensor';
import { AquariumDeviceService } from '../../../aquarium-device.service';
import { DeviceConfiguration } from '../../../models/DeviceConfiguration';
import { DeviceAction } from '../../../models/DeviceAction';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { HttpErrorResponse } from '@angular/common/http';
import { Store } from '@ngrx/store';
import { connectToDevice, connectToMixingStation } from '../../../store/device.actions';
import { DeviceSensorPolarity } from '../../../models/DeviceSensorPolarity';
import { WirelessDevice } from '../../../models/WirelessDevice';

@Component({
  selector: 'device-wireless-device-upsert-modal',
  templateUrl: './device-wireless-device-upsert-modal.component.html',
  styleUrls: []
})
export class DeviceWirelessDeviceUpsertModalComponent implements OnInit {
  public faTrashAlt = faTrashAlt;
  public disabled: boolean;
  public error: string;

  public configuredDevice: DeviceConfiguration;
  public wirelessDevice: WirelessDevice = new WirelessDevice();

  public readSensorChecked: boolean;
  public maxRunTimeChecked: boolean;

  public startTime: string;
  public endTime: string;

  constructor(@Inject(MAT_DIALOG_DATA) private data,
    private _self: MatDialogRef<DeviceWirelessDeviceUpsertModalComponent>,
    private store: Store,
    private _aquariumService: AquariumDeviceService) {

    if (data.configuredDevice)
      this.configuredDevice = data.configuredDevice;
    if (data.wirelessDevice && data.wirelessDevice.id) {
      this.wirelessDevice = { ...data.wirelessDevice }
    }


  }
  public ngOnInit(): void {

  }
  public clickFinishTask() {
    this.disabled = true;
    this._aquariumService.upsertMixingStation(this.wirelessDevice).subscribe((newConfiguration: DeviceConfiguration) => {
      this.store.dispatch(connectToDevice());
      this._self.close();
    }, (err: HttpErrorResponse) => {
      this.disabled = false;
      this._self.disableClose = false;
      this.error = err.message;
    });
  }
  public clickRemoveTask() {
    this.disabled = true;
    this._aquariumService.disconnectMixingStation(this.wirelessDevice).subscribe((newConfiguration: DeviceConfiguration) => {
      this.store.dispatch(connectToDevice());
      this._self.close();
    }, (err: HttpErrorResponse) => {
      this.disabled = false;
      this._self.disableClose = false;
      this.error = err.message;
    });
  }
}
