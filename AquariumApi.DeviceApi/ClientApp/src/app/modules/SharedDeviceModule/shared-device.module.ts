//This module should be in sync with AquariumDashboard and AquariumApi.DeviceApi
import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatDialogModule } from "@angular/material/dialog";
import { MatInputModule } from "@angular/material/input";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatRadioModule } from "@angular/material/radio";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { MatSelectModule } from '@angular/material/select';
import { DeviceConfigurationCardComponent } from "./components/device-configuration-card/device-configuration-card.component";
import { DeviceSensorListComponent } from "./components/device-sensor-list/device-sensor-list.component";
import { DeviceSensorGpioBoardComponent } from "./components/device-sensor-gpio-board/device-sensor-gpio-board.component";
import {MatTooltipModule} from '@angular/material/tooltip';
import { DeviceMixingStationSearchComponent } from "./components/device-mixing-station-search/device-mixing-station-search.component";
import { DeviceLoadingSpinnerComponent } from "./components/device-loading-spinner/device-loading-spinner.component";
import { DeviceSensorListItemComponent } from "./components/device-sensor-list-item/device-sensor-list-item.component";
import { DeviceSensorTestButtonComponent } from "./components/device-sensor-test-button/device-sensor-test-button.component";
import { DeviceGenericSelectComponent } from "./components/device-generic-select/device-generic-select.component";
import { DeviceSensorTestModalComponent } from "./components/modals/device-sensor-test-modal/device-sensor-test-modal.component";
import { DeviceSensorUpsertModalComponent } from "./components/modals/device-sensor-upsert-modal/device-sensor-upsert-modal.component";
import { DeviceSensorUpsertButtonComponent } from "./components/device-sensor-upsert-button/device-sensor-upsert-button.component";
import { DeviceMixingStationListItemComponent } from "./components/device-mixing-station-list-item/device-mixing-station-list-item.component";
import { DeviceMixingStationConfigurationComponent } from "./components/device-mixing-station-configuration/device-mixing-station-configuration.component";
import { DeviceMixingStationDisconnectButtonComponent } from "./components/device-mixing-station-disconnect-button/device-mixing-station-disconnect-button.component";
import { StoreModule } from "@ngrx/store";
import { deviceReducer } from "./store/device.reducer";
import { DeviceMixingStationSensorListItemComponent } from "./components/device-mixing-station-sensor-list-item/device-mixing-station-sensor-list-item.component";
import { DeviceMixingStationStopButtonComponent } from "./components/device-mixing-station-stop-button/device-mixing-station-stop-button.component";
import { DeviceScheduleUpsertButtonComponent } from "./components/device-schedule-upsert-button/device-schedule-upsert-button.component";
import { DeviceTaskListItemComponent } from "./components/device-task-list-item/device-task-list-item.component";
import { DeviceScheduleUpsertModalComponent } from "./components/modals/device-schedule-upsert-modal/device-schedule-upsert-modal.component";
import { DeviceDateTimeSelectComponent } from "./components/device-date-time-select/device-date-time-select.component";
import { DeviceGenericObjectSelectComponent } from "./components/device-generic-object-select/device-generic-object-select.component";
import { DeviceTaskUpsertModalComponent } from "./components/modals/device-task-upsert-modal/device-task-upsert-modal.component";
import { DeviceTaskUpsertButtonComponent } from "./components/device-task-upsert-button/device-task-upsert-button.component";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { DeviceTaskListComponent } from "./components/device-task-list/device-task-list.component";
import { DeviceTaskActionListItemComponent } from "./components/device-task-action-list-item/device-task-action-list-item.component";
import { DeviceTaskPerformButtonComponent } from "./components/device-task-perform-button/device-task-perform-button.component";
import { DeviceScheduleTaskListItemComponent } from "./components/device-schedule-task-list-item/device-schedule-task-list-item.component";
import { DeviceScheduleListComponent } from "./components/device-schedule-list/device-schedule-list.component";
import { DeviceScheduleListItemComponent } from "./components/device-schedule-list-item/device-schedule-list-item.component";
import { ConfirmModalComponent } from "./components/modals/confirm-modal/confirm-modal.component";
import { EffectsModule } from "@ngrx/effects";
import { DeviceEffects } from "./store/device.effects";
import { DeviceMixingStationSensorListComponent } from "./components/device-mixing-station-sensor-list/device-mixing-station-sensor-list.component";
import { DeviceSensorGpioBoardPinComponent } from "./components/device-sensor-gpio-board-pin/device-sensor-gpio-board-pin.component";
import { DeviceScheduleStatusComponent } from "./components/device-schedule-status/device-schedule-status.component";
import { DeviceScheduledJobListItemComponent } from "./components/device-scheduled-job-list-item/device-scheduled-job-list-item.component";
import { DeviceScheduledJobListComponent } from "./components/device-scheduled-job-list/device-scheduled-job-list.component";
import { ToastrModule } from "ngx-toastr";

@NgModule({
    imports: [
        CommonModule,
        FontAwesomeModule,
        ReactiveFormsModule,
        FormsModule,
        MatInputModule,
        MatDialogModule,
        MatProgressSpinnerModule,
        MatRadioModule,
        MatSelectModule,
        MatTooltipModule,
        MatCheckboxModule,

        //State management
        StoreModule.forFeature("device",deviceReducer),
        EffectsModule.forFeature([DeviceEffects]),
        ToastrModule.forRoot(),
    ],
    exports: [
        DeviceSensorUpsertButtonComponent,
        DeviceScheduleUpsertButtonComponent,
        DeviceScheduleStatusComponent,
        DeviceTaskUpsertButtonComponent,
        DeviceSensorTestButtonComponent,
        DeviceConfigurationCardComponent,
        DeviceSensorListItemComponent,
        DeviceSensorListComponent,
        DeviceSensorGpioBoardComponent,
        DeviceTaskListComponent,
        DeviceScheduleListComponent,
        DeviceMixingStationSearchComponent,
        DeviceMixingStationListItemComponent,
        DeviceLoadingSpinnerComponent,
        DeviceMixingStationDisconnectButtonComponent,
        DeviceMixingStationConfigurationComponent,
        DeviceMixingStationStopButtonComponent,
        ConfirmModalComponent,
    ],
    declarations: [
        DeviceGenericSelectComponent,
        DeviceLoadingSpinnerComponent,
        DeviceGenericObjectSelectComponent,
        DeviceDateTimeSelectComponent,
        ConfirmModalComponent,

        //Sensors
        DeviceSensorUpsertButtonComponent,
        DeviceSensorTestButtonComponent,
        DeviceSensorListItemComponent,
        DeviceSensorListComponent,
        DeviceSensorGpioBoardComponent,
        DeviceSensorGpioBoardPinComponent,

        //Schedule
        DeviceScheduleListComponent,
        DeviceScheduleListItemComponent,
        DeviceScheduleUpsertButtonComponent,
        DeviceScheduleTaskListItemComponent,
        DeviceTaskListComponent,
        DeviceTaskListItemComponent,
        DeviceTaskUpsertButtonComponent,
        DeviceTaskPerformButtonComponent,
        DeviceTaskActionListItemComponent,
        DeviceScheduleStatusComponent,
        DeviceScheduledJobListItemComponent,
        DeviceScheduledJobListComponent,

        //MixingStation
        DeviceMixingStationDisconnectButtonComponent,
        DeviceMixingStationSearchComponent,
        DeviceMixingStationListItemComponent,
        DeviceMixingStationConfigurationComponent,
        DeviceMixingStationSensorListItemComponent,
        DeviceMixingStationStopButtonComponent,
        DeviceMixingStationSensorListComponent,
        
        //Settings
        DeviceConfigurationCardComponent,

        //Modals
        DeviceSensorUpsertModalComponent,
        DeviceSensorTestModalComponent,
        DeviceScheduleUpsertModalComponent,
        DeviceTaskUpsertModalComponent,
    ],
    providers: [
    ],
})
export class SharedDeviceModule { }

