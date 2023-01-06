import { Component, Input } from '@angular/core';

import { MatDialog } from '@angular/material/dialog';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceScheduleTask } from '../../models/DeviceScheduleTask';
import { Store } from '@ngrx/store';
import { deviceGetScheduleStatus, deviceGetSensorValues } from '../../store/device.actions';
import { ToastrService } from 'ngx-toastr';

@Component({
    selector: 'device-task-perform-button',
    templateUrl: './device-task-perform-button.component.html',
    styleUrls: []
})
export class DeviceTaskPerformButtonComponent {
    @Input() task: DeviceScheduleTask;
    @Input() disabled: boolean;

    public faTrash = faTrash;
    constructor(private dialog: MatDialog, private store: Store, private service: AquariumDeviceService, private notifier: ToastrService) {
    }
    public clickAction() {
        this.service.performScheduleTask(this.task).subscribe((data) => {
            this.notifier.success("Task performed!");
            this.store.dispatch(deviceGetScheduleStatus());
            this.store.dispatch(deviceGetSensorValues());
        }, err => {
            this.notifier.error("Could not perform device task...");
            console.error(err);
        });
    }
}
