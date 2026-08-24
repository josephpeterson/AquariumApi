import { Component, Input, Inject, ElementRef, ViewContainerRef } from '@angular/core';

import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DeviceSensor } from 'src/app/modules/SharedDeviceModule/models/DeviceSensor';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceSensorUpsertModalComponent } from '../modals/device-sensor-upsert-modal/device-sensor-upsert-modal.component';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { DeviceSensorTestModalComponent } from '../modals/device-sensor-test-modal/device-sensor-test-modal.component';
import { GpioPinValue } from '../../models/GpioPinValue';
import { DeviceSensorPolarity } from '../../models/DeviceSensorPolarity';

@Component({
    selector: 'device-sensor-test-button',
    templateUrl: './device-sensor-test-button.component.html',
    styleUrls: ['./device-sensor-test-button.component.scss']
})
export class DeviceSensorTestButtonComponent {
    @Input() sensor: DeviceSensor;
    @Input() disabled: boolean;
    @Input() inline: any = null;

    public faTrash = faTrash;
    constructor(private dialog: MatDialog) {
    }
    public clickAction() {
        if (this.disabled) return;
        var dialog = this.dialog.open(DeviceSensorTestModalComponent, {
            maxWidth: 500,
            data: {
                sensor: this.sensor
            }
        });
    }
    DeviceSensorPolarity = DeviceSensorPolarity;
    public GpioPinValue = GpioPinValue;

}
