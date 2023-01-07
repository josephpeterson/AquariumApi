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
import { DashboardContainerComponent } from "./components/containers/dashboard/dashboard-container.component";
import { AquariumFormComponent } from "./components/containers/login/aquarium-form/aquarium-form.component";
import { DeviceUnavailableContainerComponent } from "./components/containers/login/device-unavailable-container.component";
import { LoginFormComponent } from "./components/containers/login/login-form/login-form.component";
import { LogPageComponent } from "./components/containers/logs/log-page.component";
import { ScheduleContainerComponent } from "./components/containers/schedule/schedule-container.component";
import { SensorsContainerComponent as DeviceSensorsContainerComponent } from "./components/containers/sensors/sensors-container.component";
import { DeviceSettingsContainerComponent } from "./components/containers/settings/settings-container.component";
import { DeviceLoginButton } from "./components/shared/login-button/login-button.component.component";
import { DeviceLoginModalComponent } from "./components/shared/login-modal/login-modal.component";
import { NavBarComponent as DeviceNavBarComponent } from "./components/shared/nav-bar/nav-bar.component";
import { DeviceSettingsPrivateListComponent } from "./components/shared/device-settings-private-list/device-settings-private-list.component";


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
    

    //Sensors
    DeviceSensorsContainerComponent,

    //Schedule
    ScheduleContainerComponent,

    //Settings
    DeviceSettingsContainerComponent,
    DeviceSettingsPrivateListComponent,
    
    //Misc.
    DeviceUnavailableContainerComponent,

    //Shared
    DeviceNavBarComponent,
    DeviceLoginButton,
    DeviceLoginModalComponent,

    //Obsolete
    ExceptionListComponent,
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

