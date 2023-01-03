import { Component, OnInit, Inject } from '@angular/core';

import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DeviceSensor } from 'src/app/modules/SharedDeviceModule/models/DeviceSensor';
import { DeviceConnectionStatus, RaspberryPiModels } from 'src/app/modules/SharedDeviceModule/models/RaspberyPiModels';
import { HttpErrorResponse } from '@angular/common/http';
import { ConfirmModalComponent } from '../confirm-modal/confirm-modal.component';
import { take } from 'rxjs/operators';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { GpioPinTypes } from 'src/app/modules/SharedDeviceModule/models/GpioPinTypes';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { RaspberryPiUtils } from '../../../models/RaspberryPiUtils';
import { Store } from '@ngrx/store';
import { connectToDevice, deviceConnectionSuccess } from '../../../store/device.actions';
import { DeviceSensorPolarity } from '../../../models/DeviceSensorPolarity';
import { DeviceSensorTypes } from '../../../models/DeviceSensorTypes';
import { selectMixingStationConnection, selectMixingStationStatus } from '../../../store/device.selectors';
import { AquariumMixingStationStatus } from '../../../models/AquariumMixingStationStatus';

@Component({
  selector: 'device-sensor-upsert-modal',
  templateUrl: './device-sensor-upsert-modal.component.html',
  styleUrls: []
})
export class DeviceSensorUpsertModalComponent implements OnInit {
  public mixingStationStatus$ = this.store.select(selectMixingStationConnection);
  public mixingStationConnectionStatus$ = this.store.select(selectMixingStationStatus);
  public configuredDevice: DeviceConfiguration;
  public GpioPinTypes: typeof GpioPinTypes = GpioPinTypes;
  public newDeviceSensor: DeviceSensor = new DeviceSensor();
  public loading: boolean = false;
  public disabled: boolean = false;
  public error: string;

  public faTrash = faTrash;

  constructor(@Inject(MAT_DIALOG_DATA) private data,
    private dialog: MatDialog,
    private store: Store,
    private _self: MatDialogRef<DeviceSensorUpsertModalComponent>,
    private service: AquariumDeviceService) {
    if (data.sensor)
      this.newDeviceSensor = { ...data.sensor };
    if (data.configuredDevice)
      this.configuredDevice = data.configuredDevice;

  }
  ngOnInit() {
  }
  isBoardSupported() {
    return RaspberryPiUtils.getGpioConfiguration(this.configuredDevice.boardType) != null;
  }
  clickEditSensor() {
    this.loading = true;
    delete this.error;

    this.service.updateDeviceSensor(this.newDeviceSensor).subscribe(res => {
      this.loading = false;
      this._self.close(this.newDeviceSensor);
      this.store.dispatch(connectToDevice());
    }, (err: HttpErrorResponse) => {
      this.loading = false;
      this.error = err.message;
    });
  }
  clickRemoveSensor() {
    if (this.disabled) return;
    this.disabled = true;
    this._self.disableClose = true;

    var dialog = this.dialog.open(ConfirmModalComponent, {
      disableClose: true
    });
    dialog.componentInstance.title = "Delete Sensor";
    dialog.componentInstance.body = "Are you sure you want to remove this sensor? This will remove this sensor from any schedules and tasks that require this sensor. This action cannot be undone.";
    dialog.afterClosed().subscribe((confirm: boolean) => {
      this.disabled = false;
      this._self.disableClose = false;
      if (confirm) {
        this.disabled = true;
        this._self.disableClose = true;


        delete this.error;

        this.service.deleteDeviceSensor(this.newDeviceSensor).subscribe(res => {
          this.disabled = false;
          this._self.close(null);
          this.store.dispatch(connectToDevice());
        }, (err: HttpErrorResponse) => {
          this.disabled = false;
          this._self.disableClose = false;
          this.error = err.message;
        });
      }

    });
  }
  getMixingStationSensorByPin(pin: number, status: AquariumMixingStationStatus) {
    if (isNaN(pin))
      return null;
    return status.valves.filter(v => v.pin == pin)[0];
  }
  DeviceSensorPolarity = DeviceSensorPolarity;
  DeviceSensorTypes = DeviceSensorTypes;
  DeviceConnectionStatus = DeviceConnectionStatus;
}
