import { Component, Input, Inject, ElementRef, ViewContainerRef } from '@angular/core';

import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DeviceSensor } from 'src/app/modules/SharedDeviceModule/models/DeviceSensor';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceSensorUpsertModalComponent } from '../modals/device-sensor-upsert-modal/device-sensor-upsert-modal.component';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice } from '../../store/device.selectors';

@Component({
    selector: 'device-sensor-upsert-button',
    templateUrl: './device-sensor-upsert-button.component.html',
    styleUrls: []
})
export class DeviceSensorUpsertButtonComponent {
    public configuredDevice$ = this.store.select(selectConfiguredDevice);
    @Input() sensor: DeviceSensor;
    @Input() disabled: boolean;

    public faTrash = faTrash;
    constructor(private dialog: MatDialog,
        private store: Store) {
    }
    public clickAction(device) {
        var dialog = this.dialog.open(DeviceSensorUpsertModalComponent, {
            width: "40%",
            data: {
                sensor: this.sensor,
                configuredDevice: device
            }
        });
    }
}
