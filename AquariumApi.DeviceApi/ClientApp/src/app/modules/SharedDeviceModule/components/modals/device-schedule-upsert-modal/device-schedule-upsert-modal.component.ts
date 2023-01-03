import { Component, OnInit, Input, Inject } from '@angular/core';
import { DeviceScheduleTask } from 'src/app/modules/SharedDeviceModule/models/DeviceScheduleTask';
import { DeviceSchedule } from 'src/app/modules/SharedDeviceModule/models/DeviceSchedule';

import { MatDialogRef, MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faPlus, faTrash, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { NotificationService } from 'src/app/services/notification.service';
import { AquariumDeviceService } from '../../../aquarium-device.service';
import * as moment from 'moment';
import { DeviceConfiguration } from '../../../models/DeviceConfiguration';
import { HttpErrorResponse } from '@angular/common/http';
import { Store } from '@ngrx/store';
import { connectToDevice } from '../../../store/device.actions';

@Component({
  selector: 'device-schedule-upsert-modal',
  templateUrl: './device-schedule-upsert-modal.component.html',
  styleUrls: []
})
export class DeviceScheduleUpsertModalComponent implements OnInit {

  public faTrash = faTrash;
  public error: string;
  public disabled: boolean;
  public configuredDevice: DeviceConfiguration;
  public schedule: DeviceSchedule = new DeviceSchedule();

  constructor(
    @Inject(MAT_DIALOG_DATA) private data,
    private _aquariumService: AquariumDeviceService,
    private store: Store,
    private _notifier: NotificationService,
    private _dialog: MatDialog,
    private _dialogRef: MatDialogRef<DeviceScheduleUpsertModalComponent>) {
    if (data.schedule && data.schedule.id) {
      this.schedule = DeviceSchedule.expandSchedule(data.schedule, data.configuredDevice);
    }
    if (data.configuredDevice)
      this.configuredDevice = data.configuredDevice;
  }

  ngOnInit() {
    if (this.schedule.tasks.length == 0) {
      this.addPlaceholderTask();
    }
    //apply filters based on task assignment
    if (this.schedule.dateConditions && this.schedule.dateConditions.length == 7)
      this.filters.forEach((f, i) => {
        if (this.schedule.dateConditions[i] == "1")
          f.value = true;
        else
          f.value = false;
      });
    else
      this.schedule.dateConditions = "1111111";

    if (!this.schedule.startTime)
      this.schedule.startTime = moment().utc().format();
  }
  public addPlaceholderTask() {
    this.schedule.tasks.push(new DeviceScheduleTask());
  }

  public clickRemoveTask(task: DeviceScheduleTask) {
    var tasks = this.schedule.tasks.filter(t => t != task);
    this.schedule.tasks = tasks;
  }
  public clickSaveSchedule() {
    this.disabled = true;
    console.log("Saving schedule:", this.schedule);
    this._aquariumService.upsertDeviceSchedule(this.schedule).subscribe((data: DeviceSchedule[]) => {
      this._dialogRef.close(data);
      this.store.dispatch(connectToDevice());
    }, (err: HttpErrorResponse) => {
      this.disabled = false;
      this.error = err.error;
      console.error(err);
    });
  }
  public clickDeleteSchedule() {
    this.disabled = true;
    this._aquariumService.deleteDeviceSchedule(this.schedule).subscribe(data => {
      this._dialogRef.close(data);
      this.store.dispatch(connectToDevice());
    }, err => {
      this.disabled = false;
      this._notifier.notify("error", "Could not delete device schedule...");
      console.error(err);
    });
  }
  public clickAddTask() {
    this.addPlaceholderTask();
  }


  public filters = [
    { name: "Sunday", value: true },
    { name: "Monday", value: true },
    { name: "Tuesday", value: true },
    { name: "Wednesday", value: true },
    { name: "Thursday", value: true },
    { name: "Friday", value: true },
    { name: "Saturday", value: true },
  ];
  public updateFilters() {
    this.schedule.dateConditions = "";
    this.filters.forEach(f => {
      this.schedule.dateConditions += f.value ? "1" : "0";
    });
  }
  public getDateConditionText() {
    var str = "This schedule will run ";
    var fs = this.filters.filter(f => f.value);
    if (fs.length == 7)
      str += "every day";
    else if (fs.length == 1)
      str += `every ${fs[0].name}`;
    else
      str += `${fs.length} times a week`;
    str += " at " + moment(this.schedule.startTime).local().format("hh:mm a");
    return str;
  }

}
