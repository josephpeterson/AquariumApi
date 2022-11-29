import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { LoginFormComponent } from './components/containers/login/login-form/login-form.component';
import { AquariumFormComponent } from './components/containers/login/aquarium-form/aquarium-form.component';
import { NavBarComponent } from './components/shared/nav-bar/nav-bar.component';
import { ScheduleInformationComponent } from './components/containers/dashboard/components/schedule-information/schedule-information.component';
import { LogPageComponent } from './components/containers/logs/log-page.component';
import { DashboardContainerComponent } from './components/containers/dashboard/dashboard-container.component';
import { ApplicationLogViewComponent } from './components/shared/application-log-view/application-log-view.component';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule } from '@angular/common/http';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatInputModule} from '@angular/material/input';
import {MatRadioModule} from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatProgressSpinnerModule, MatSnackBarModule } from '@angular/material';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { LoginContainerComponent } from './components/containers/login/login-container.component';
import { NotifierModule } from "angular-notifier";
import { DeviceNotifierConfig } from './config/DeviceNotifierConfig';
import { HardwareInformationComponent } from './components/containers/dashboard/components/hardware-information/hardware-information.component';
import { ExceptionListComponent } from './components/containers/dashboard/components/exception-list/exception-list.component';
import { DeviceSensorsComponent } from './components/containers/sensors/components/device-sensors/device-sensors.component';
import { DeviceScheduledJobsComponent } from './components/containers/schedule/components/device-scheduled-jobs/device-scheduled-jobs.component';
import { DeviceJobListItemComponent } from './components/containers/schedule/components/device-job-list-item/device-job-list-item.component';
import { NotificationDialogComponent } from './components/shared/notification-dialog/notification-dialog.component';
import { DeviceScheduledJobListItemComponent } from './components/containers/schedule/components/device-scheduled-job-list-item/device-scheduled-job-list-item.component';
import { DeviceTaskCardComponent } from './components/containers/schedule/components/device-task-card/device-task-card.component';
import { MixingStationContainer } from './components/containers/mixing-station/mixing-station-container.component';
import { DeviceMixingStationComponent } from './components/containers/mixing-station/components/device-mixing-station/device-mixing-station.component';
import { DeviceMixingStationCreateComponent } from './components/containers/mixing-station/components/device-mixing-station-create/device-mixing-station-create.component';
import { LoadingSpinnerComponent } from './components/shared/loading-spinner/loading-spinner.component';

@NgModule({
  declarations: [
    AppComponent,
    LoadingSpinnerComponent,
    LoginFormComponent,
    AquariumFormComponent,
    NavBarComponent,
    ScheduleInformationComponent,
    DeviceSensorsComponent,
    DeviceScheduledJobsComponent,
    DeviceJobListItemComponent,
    LogPageComponent,
    DashboardContainerComponent,

    MixingStationContainer,
    DeviceMixingStationComponent,
    DeviceMixingStationCreateComponent,
    
    LoginContainerComponent,
    ApplicationLogViewComponent,
    HardwareInformationComponent,
    ExceptionListComponent,
    NotificationDialogComponent,
    DeviceScheduledJobListItemComponent,
    DeviceTaskCardComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatDialogModule,
    NgbModule.forRoot(),
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    MatInputModule,
    MatRadioModule,
    MatCheckboxModule,
    MatSnackBarModule,
    NotifierModule.withConfig(DeviceNotifierConfig),
    MatProgressSpinnerModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
