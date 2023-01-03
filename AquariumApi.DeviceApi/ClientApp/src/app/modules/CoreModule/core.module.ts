import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatNativeDateModule } from "@angular/material/core";
import { MatDialogModule } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatRadioModule } from "@angular/material/radio";
import { RouterModule } from "@angular/router";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { SharedDeviceModule } from "../SharedDeviceModule/shared-device.module";
import { DeviceAquariumOverviewCardComponent } from "./components/containers/dashboard/components/aquarium-overview-card/aquarium-overview-card.component";
import { ExceptionListComponent } from "./components/containers/dashboard/components/exception-list/exception-list.component";
import { DeviceMixingStationOverviewCard } from "./components/containers/dashboard/components/mixing-station-overview-card/mixing-station-overview-card.component";
import { DashboardContainerComponent } from "./components/containers/dashboard/dashboard-container.component";
import { AquariumFormComponent } from "./components/containers/login/aquarium-form/aquarium-form.component";
import { DeviceUnavailableContainerComponent } from "./components/containers/login/device-unavailable-container.component";
import { LoginFormComponent } from "./components/containers/login/login-form/login-form.component";
import { LogPageComponent } from "./components/containers/logs/log-page.component";
import { DeviceMixingStationCreateComponent } from "./components/device-mixing-station-create/device-mixing-station-create.component";
import { MixingStationContainerComponent } from "./components/containers/mixing-station/mixing-station-container.component";
import { DeviceJobListItemComponent } from "./components/containers/schedule/components/device-job-list-item/device-job-list-item.component";
import { DeviceScheduledJobListItemComponent } from "./components/containers/schedule/components/device-scheduled-job-list-item/device-scheduled-job-list-item.component";
import { ScheduleContainerComponent } from "./components/containers/schedule/schedule-container.component";
import { SensorsContainerComponent as DeviceSensorsContainerComponent } from "./components/containers/sensors/sensors-container.component";
import { DeviceSettingsContainerComponent } from "./components/containers/settings/settings-container.component";
import { ApplicationLogViewComponent } from "./components/shared/application-log-view/application-log-view.component";
import { DeviceLoginButton } from "./components/shared/login-button/login-button.component.component";
import { DeviceLoginModalComponent } from "./components/shared/login-modal/login-modal.component";
import { NavBarComponent as DeviceNavBarComponent } from "./components/shared/nav-bar/nav-bar.component";
import { NotificationDialogComponent } from "./components/shared/notification-dialog/notification-dialog.component";


@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    SharedDeviceModule,

    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatInputModule,
    MatRadioModule,
    MatFormFieldModule,
    MatDialogModule,
    MatButtonModule,
    MatNativeDateModule,
  ],
  declarations: [
    //Dashboard
    DashboardContainerComponent,
    DeviceAquariumOverviewCardComponent,
    DeviceMixingStationOverviewCard,
    
    //Mixing Station
    MixingStationContainerComponent,
    DeviceMixingStationCreateComponent,

    //Sensors
    DeviceSensorsContainerComponent,

    //Schedule
    ScheduleContainerComponent,

    //Settings
    DeviceSettingsContainerComponent,
    
    //Misc.
    DeviceUnavailableContainerComponent,

    //Shared
    DeviceNavBarComponent,
    DeviceLoginButton,
    DeviceLoginModalComponent,

    //Obsolete
    ApplicationLogViewComponent,
    ExceptionListComponent,
    NotificationDialogComponent,
    DeviceScheduledJobListItemComponent,
    DeviceJobListItemComponent,
    AquariumFormComponent,
    LoginFormComponent,
    LogPageComponent,
  ],
  exports: [
    DeviceUnavailableContainerComponent,
    DeviceNavBarComponent,
  ],
  providers: [

  ],
})
export class CoreModule {}

