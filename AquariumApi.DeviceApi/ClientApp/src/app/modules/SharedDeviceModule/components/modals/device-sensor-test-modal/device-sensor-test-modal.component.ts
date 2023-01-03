import { Component, OnInit, Inject } from '@angular/core';

import { AquariumDevice } from 'src/app/models/AquariumDevice';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DeviceSensor } from 'src/app/modules/SharedDeviceModule/models/DeviceSensor';
import { HttpErrorResponse } from '@angular/common/http';
import { DeviceSensorTestRequest } from 'src/app/modules/SharedDeviceModule/models/DeviceSensorTestRequest';
import { NotificationService } from 'src/app/services/notification.service';
import { BaseException } from 'src/app/modules/SharedDeviceModule/models/BaseException';
import { AquariumDeviceService } from '../../../aquarium-device.service';
import { Store } from '@ngrx/store';
import { deviceGetSensorValues } from '../../../store/device.actions';

@Component({
  selector: 'device-sensor-test-modal',
  templateUrl: './device-sensor-test-modal.component.html',
  styleUrls: ['./device-sensor-test-modal.component.scss']
})
export class DeviceSensorTestModalComponent implements OnInit {
  public sensor: DeviceSensor = new DeviceSensor();
  public device: AquariumDevice = new AquariumDevice();
  public request: DeviceSensorTestRequest = new DeviceSensorTestRequest();
  public loading: boolean = false;
  public error: string;

  constructor(@Inject(MAT_DIALOG_DATA) private data,
    private _self: MatDialogRef<DeviceSensorTestModalComponent>,
    private store: Store,
    private notifier: NotificationService,
    private _aquariumService: AquariumDeviceService) {
    this.sensor = data.sensor;
  }
  ngOnInit() {

  }
  public clickRunTest() {
    this.request.sensorId = this.sensor.id;
    this._aquariumService.testDeviceSensor(this.request).subscribe(
      (request: DeviceSensorTestRequest) => {
        console.log(request);
        this.notifier.notify("success", `Successfully ran test for sensor: ${this.sensor.name}`);
        this.store.dispatch(deviceGetSensorValues());
      }, (err: HttpErrorResponse) => {
        var error = err.error as BaseException;
        console.log(error);
        this.notifier.notify("error", `Could not run test for device sensor. ${error.message}`);
      })
    this._self.close();
  }
}
