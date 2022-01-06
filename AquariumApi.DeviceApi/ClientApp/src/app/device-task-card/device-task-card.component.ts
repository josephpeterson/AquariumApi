import { Component, OnInit, Input } from '@angular/core';
import { AquariumDevice } from 'src/app/models/AquariumDevice';

import { faEdit, faMinus, faPlus, faTrash } from '@fortawesome/free-solid-svg-icons';
import { DeviceScheduleState } from 'src/app/models/DeviceScheduleState';
import * as moment from 'moment';
import { DeviceScheduleTask } from 'src/app/models/DeviceScheduleTask';
import { NotificationService } from 'src/app/services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { ClientService } from '../services/client.service';

@Component({
  selector: 'device-task-card',
  templateUrl: './device-task-card.component.html',
  //styleUrls: ['./device-task-card.component.scss']
})
export class DeviceTaskCardComponent implements OnInit {

  @Input("device") public device: AquariumDevice;
  scanning: boolean;

  faMinus = faMinus;
  faEdit = faEdit;
  faTrash = faTrash;
  pinging: boolean;
  scheduleState: DeviceScheduleState;
  performingTask: boolean;
  taskNames: any[];
  
  public faPlus = faPlus;


  constructor(public _aquariumService: ClientService,
    public _dialog: MatDialog,
    public notifier: NotificationService) { }

  ngOnInit() {

  }

  clickPerformTask(task: DeviceScheduleTask) {
    this.performingTask = true;
    this._aquariumService.performScheduleTask(task).subscribe(
      (data) => {
        this.performingTask = false;
        this.notifier.notify("success", "Task was performed successfully!");
      }, err => {
        this.performingTask = false;
        this.notifier.notify("error", "Task: " + err.error);
      })
  }
  public readableDuration(task: DeviceScheduleTask) {
    return;
    /*
    var d = moment(task.startTime).diff(moment());
    return moment.duration(d).humanize();
    */
  }
  public readableDate(date: string) {
    return moment(date).local().calendar();
  }
}
