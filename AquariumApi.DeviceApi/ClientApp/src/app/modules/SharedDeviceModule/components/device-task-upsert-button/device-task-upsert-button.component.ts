import { Component, Input, Inject, ElementRef, ViewContainerRef } from '@angular/core';

import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DeviceSensor } from 'src/app/modules/SharedDeviceModule/models/DeviceSensor';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceSensorUpsertModalComponent } from '../modals/device-sensor-upsert-modal/device-sensor-upsert-modal.component';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { DeviceTaskUpsertModalComponent } from '../modals/device-task-upsert-modal/device-task-upsert-modal.component';
import { DeviceScheduleTask } from '../../models/DeviceScheduleTask';

@Component({
    selector: 'device-task-upsert-button',
    templateUrl: './device-task-upsert-button.component.html',
    styleUrls: []
})
export class DeviceTaskUpsertButtonComponent {
    public configuredDevice$ = this.store.select(selectConfiguredDevice);
    @Input() task: DeviceScheduleTask;
    @Input() disabled: boolean;

    public faTrash = faTrash;
    constructor(private dialog: MatDialog,
        private store: Store) {
    }
    public clickAction(device) {
        var dialog = this.dialog.open(DeviceTaskUpsertModalComponent, {
            data: {
                task: this.task,
                configuredDevice: device
            }
        });
    }
}
