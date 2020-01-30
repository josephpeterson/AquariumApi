import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { LoginFormComponent } from './containers/login/login-form/login-form.component';
import { AquariumFormComponent } from './containers/login/aquarium-form/aquarium-form.component';
import { NavBarComponent } from './shared/nav-bar/nav-bar.component';
import { ScheduleInformationComponent } from './schedule-information/schedule-information.component';
import { LogPageComponent } from './containers/logs/log-page.component';
import { DashboardContainerComponent } from './containers/dashboard/dashboard-container.component';
import { ApplicationLogViewComponent } from './shared/application-log-view/application-log-view.component';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule } from '@angular/common/http';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatInputModule} from '@angular/material/input';
import {MatRadioModule} from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinnerModule } from '@angular/material';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { LoginContainerComponent } from './containers/login/login-container.component';
import { NotifierModule } from "angular-notifier";
import { DeviceNotifierConfig } from './config/DeviceNotifierConfig';

@NgModule({
  declarations: [
    AppComponent,
    LoginFormComponent,
    AquariumFormComponent,
    NavBarComponent,
    ScheduleInformationComponent,
    LogPageComponent,
    DashboardContainerComponent,
    LoginContainerComponent,
    ApplicationLogViewComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule.forRoot(),
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    MatInputModule,
    MatRadioModule,
    MatCheckboxModule,
    NotifierModule.withConfig(DeviceNotifierConfig),
    MatProgressSpinnerModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
