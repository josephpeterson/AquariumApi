import { Component } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Store } from "@ngrx/store";
import { AquariumDeviceService } from "src/app/modules/SharedDeviceModule/aquarium-device.service";
import { ConfirmModalComponent } from "src/app/modules/SharedDeviceModule/components/modals/confirm-modal/confirm-modal.component";
import { DeviceConfiguration } from "src/app/modules/SharedDeviceModule/models/DeviceConfiguration";
import { connectToDevice } from "src/app/modules/SharedDeviceModule/store/device.actions";
import { selectConfiguredDevice } from "src/app/modules/SharedDeviceModule/store/device.selectors";

@Component({
    selector: 'device-settings-private-list',
    templateUrl: './device-settings-private-list.component.html',
    styleUrls: [],
})
export class DeviceSettingsPrivateListComponent {
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
        this.configuredDevice.privateSettings = { ...this.originalDevice.privateSettings };
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