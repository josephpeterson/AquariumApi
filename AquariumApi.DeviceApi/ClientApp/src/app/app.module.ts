import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginFormComponent } from './login-form/login-form.component';
import { AquariumFormComponent } from './aquarium-form/aquarium-form.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatInputModule} from '@angular/material/input';
import {MatRadioModule} from '@angular/material/radio';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { InformationPageComponent } from './information-page/information-page.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { ScheduleInformationComponent } from './schedule-information/schedule-information.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginFormComponent,
    AquariumFormComponent,
    InformationPageComponent,
    NavBarComponent,
    ScheduleInformationComponent
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
    MatProgressSpinnerModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
