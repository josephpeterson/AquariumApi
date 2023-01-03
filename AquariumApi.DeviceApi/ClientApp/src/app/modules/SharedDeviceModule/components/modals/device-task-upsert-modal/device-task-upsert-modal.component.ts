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
import { connectToDevice } from '../../../store/device.actions';
import { DeviceSensorPolarity } from '../../../models/DeviceSensorPolarity';

@Component({
  selector: 'device-task-upsert-modal',
  templateUrl: './device-task-upsert-modal.component.html',
  styleUrls: []
})
export class DeviceTaskUpsertModalComponent implements OnInit {
  public faTrashAlt = faTrashAlt;
  public loading: boolean;
  public error: string;

  public configuredDevice: DeviceConfiguration;
  public newTask: DeviceScheduleTask = new DeviceScheduleTask();

  public readSensorChecked: boolean;
  public maxRunTimeChecked: boolean;

  public startTime: string;
  public endTime: string;

  constructor(@Inject(MAT_DIALOG_DATA) private data,
    private _self: MatDialogRef<DeviceTaskUpsertModalComponent>,
    private store: Store,
    private _aquariumService: AquariumDeviceService) {

    if (data.configuredDevice)
      this.configuredDevice = data.configuredDevice;
    if (data.task && data.task.id) {
      this.newTask = DeviceScheduleTask.expandTask(data.task, data.configuredDevice);
    }


  }
  public ngOnInit(): void {
    console.log(this.newTask);
    if (this.newTask.actions.length == 0)
      this.clickAddAction();
    if (this.newTask.triggerSensorId)
      this.readSensorChecked = true;
  }
  public clickFinishTask() {
    this.validate();
    if (this.error) return;

    this.loading = true;

    console.log("Saving task:", this.newTask);
    //set up data
    if (!this.readSensorChecked) {
      this.newTask.triggerSensor = new DeviceSensor();
      delete this.newTask.triggerSensorId;
      delete this.newTask.triggerSensorValue;
    }

    this._aquariumService.upsertDeviceTask(this.newTask).subscribe(deviceTask => {
      this._self.close(deviceTask);
      this.loading = false;
      this.store.dispatch(connectToDevice());
    },
      (err: HttpErrorResponse) => {
        console.error("Error creating new device task:", err);
        this.loading = false;
        this.error = err.error;
      });
  }
  public validate() {
    delete this.error;
    return true;
  }
  public clickAddAction() {
    this.newTask.actions.push(new DeviceAction());
  }
  public clickRemoveAction(action) {
    if (this.newTask.actions.length > 1)
      this.newTask.actions = this.newTask.actions.filter(x => x != action);
  }
  public clickRemoveTask() {
    this.loading = true;
    this._aquariumService.deleteDeviceTask(this.newTask).subscribe(deviceTask => {
      this._self.close(deviceTask);
      this.loading = false;
      this.store.dispatch(connectToDevice());
    },
      (err: HttpErrorResponse) => {
        console.error("Error creating removing device task:", err);
        this.loading = false;
        this.error = err.error;
      });
  }
  public getReadableSensors() {
    return this.configuredDevice.sensors.filter(s => s.polarity == DeviceSensorPolarity.Input)
  }
}
