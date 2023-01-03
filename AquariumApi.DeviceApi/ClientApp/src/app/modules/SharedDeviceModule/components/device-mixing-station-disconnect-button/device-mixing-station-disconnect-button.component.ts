import { Component, Input, Inject, ElementRef, ViewContainerRef } from '@angular/core';

import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DeviceSensor } from 'src/app/modules/SharedDeviceModule/models/DeviceSensor';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { AquariumDeviceService } from 'src/app/modules/SharedDeviceModule/aquarium-device.service';
import { DeviceSensorUpsertModalComponent } from '../modals/device-sensor-upsert-modal/device-sensor-upsert-modal.component';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { Store } from '@ngrx/store';
import { connectToDevice, connectToMixingStation, deviceMixingStationConnectionSuccess, disconnectMixingStation } from '../../store/device.actions';

@Component({
    selector: 'device-mixing-station-disconnect-button',
    templateUrl: './device-mixing-station-disconnect-button.component.html',
    styleUrls: []
})
export class DeviceMixingStationDisconnectButtonComponent {
    @Input() sensor: DeviceSensor;
    @Input() disabled: boolean;

    public faTrash = faTrash;
    constructor(private dialog: MatDialog,
        private store: Store,
        private service: AquariumDeviceService) {
    }
    public clickAction() {
        this.service.disconnectMixingStation().subscribe(data => {
            this.store.dispatch(connectToDevice());
            this.store.dispatch(disconnectMixingStation());
            //this.store.dispatch(deviceMixingStationConnectionSuccess(undefined));
        });
    }
}
