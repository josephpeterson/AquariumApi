import { Component } from '@angular/core';
import { AquariumDeviceService } from '../../aquarium-device.service';
import { DeviceConfiguration } from 'src/app/modules/SharedDeviceModule/models/DeviceConfiguration';
import { Store } from '@ngrx/store';
import { selectConfiguredDevice } from '../../store/device.selectors';
import { connectToDevice } from '../../store/device.actions';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmModalComponent } from '../modals/confirm-modal/confirm-modal.component';

@Component({
    selector: 'device-configuration-card',
    templateUrl: './device-configuration-card.component.html',
    styleUrls: [],
})
export class DeviceConfigurationCardComponent {
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
    clickFactoryReset() {
        this.dialog.open(ConfirmModalComponent, {
            data: {
                title: "Factory Reset",
                body: "Are you sure you would like to factory reset this device? This action cannot be undone."
            }
        }).afterClosed().subscribe(d => {
            if (d) { 
                this.service.factoryReset().subscribe(d => {
                    this.store.dispatch(connectToDevice());
                })
            }
        });
    }
    hasChanges() {
        return JSON.stringify(this.configuredDevice) !== JSON.stringify(this.originalDevice);
    }
}