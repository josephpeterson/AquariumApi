import { Component, Input } from '@angular/core';

import { MatDialog } from '@angular/material/dialog';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { DeviceScheduleUpsertModalComponent } from '../modals/device-schedule-upsert-modal/device-schedule-upsert-modal.component';
import { DeviceSchedule } from '../../models/DeviceSchedule';

@Component({
    selector: 'device-schedule-upsert-button',
    templateUrl: './device-schedule-upsert-button.component.html',
    styleUrls: []
})
export class DeviceScheduleUpsertButtonComponent {
    public configuredDevice$ = this.store.select(selectConfiguredDevice);
    @Input() schedule: DeviceSchedule;
    @Input() disabled: boolean;

    public faTrash = faTrash;
    constructor(private dialog: MatDialog,
        private store: Store) {

           
    }
    public clickAction(device) {
        var dialog = this.dialog.open(DeviceScheduleUpsertModalComponent, {
            data: {
                schedule: this.schedule,
                configuredDevice: device
            }
        });
    }
}
