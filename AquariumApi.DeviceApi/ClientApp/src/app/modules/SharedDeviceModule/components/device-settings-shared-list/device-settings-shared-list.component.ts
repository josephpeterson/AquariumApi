import { Component } from '@angular/core';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { connectToDevice } from '../../store/device.actions';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmModalComponent } from '../modals/confirm-modal/confirm-modal.component';

@Component({
    selector: 'device-settings-shared-list',
    templateUrl: './device-settings-shared-list.component.html',
    styleUrls: [],
})
export class DeviceSettingsSharedListComponent {
    public configuredDevice$ = this.store.select(selectConfiguredDevice);
    public originalDevice: DeviceConfiguration;
    public configuredDevice: DeviceConfiguration;
    public loading: boolean = false;
    public disabled: boolean = false;
    public error: string;
    constructor(
        private store: Store,
        private dialog: MatDialog,
        private service: AquariumDeviceService) {
    }
    ngOnInit() {
        this.configuredDevice$.subscribe(x => {
            this.originalDevice = x;
            this.clickUndoChanges();
        });
    }
    clickUndoChanges() {
        this.configuredDevice = { ...this.originalDevice };
        this.configuredDevice.settings = { ...this.originalDevice.settings };
    }
    clickSaveChanges() {
        this.loading = true;
        this.service.applyDeviceConfiguration(this.configuredDevice).subscribe((data: DeviceConfiguration) => {
            this.loading = false;
            if (data) {
                this.store.dispatch(connectToDevice());
                //this.clickUndoChanges();
            }
            else
                console.warn("No device configuration sent back from server...");
        });
    }
    hasChanges() {
        return JSON.stringify(this.configuredDevice) !== JSON.stringify(this.originalDevice);
    }
}