import { Component, Input, Inject, ElementRef, ViewContainerRef } from '@angular/core';

import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DeviceSensor } from 'src/app/modules/SharedDeviceModule/models/DeviceSensor';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceSensorUpsertModalComponent } from '../modals/device-sensor-upsert-modal/device-sensor-upsert-modal.component';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { DeviceSensorTestModalComponent } from '../modals/device-sensor-test-modal/device-sensor-test-modal.component';
import { DeviceScheduleTask } from '../../models/DeviceScheduleTask';
import { DeviceTaskUpsertModalComponent } from '../modals/device-task-upsert-modal/device-task-upsert-modal.component';
import { NotificationService } from 'src/app/services/notification.service';
import { Store } from '@ngrx/store';
import { deviceGetScheduleStatus, deviceGetSensorValues } from '../../store/device.actions';

@Component({
    selector: 'device-task-perform-button',
    templateUrl: './device-task-perform-button.component.html',
    styleUrls: []
})
export class DeviceTaskPerformButtonComponent {
    @Input() task: DeviceScheduleTask;
    @Input() disabled: boolean;

    public faTrash = faTrash;
    constructor(private dialog: MatDialog, private store: Store, private service: AquariumDeviceService, private notifier: NotificationService) {
    }
    public clickAction() {
        this.service.performScheduleTask(this.task).subscribe((data) => {
            this.notifier.notify("success", "Task performed!");
            this.store.dispatch(deviceGetScheduleStatus());
            this.store.dispatch(deviceGetSensorValues());
        }, err => {
            this.notifier.notify("error", "Could not perform device task...");
            console.error(err);
        });
    }
}
