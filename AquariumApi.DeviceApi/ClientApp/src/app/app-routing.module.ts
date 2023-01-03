import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LogPageComponent as LogContainerComponent } from './modules/CoreModule/components/containers/logs/log-page.component';
import { DashboardContainerComponent } from './modules/CoreModule/components/containers/dashboard/dashboard-container.component';
import { MixingStationContainerComponent } from './modules/CoreModule/components/containers/mixing-station/mixing-station-container.component';
import { DeviceSettingsContainerComponent } from './modules/CoreModule/components/containers/settings/settings-container.component';
import { SensorsContainerComponent } from './modules/CoreModule/components/containers/sensors/sensors-container.component';
import { ScheduleContainerComponent } from './modules/CoreModule/components/containers/schedule/schedule-container.component';

const routes: Routes = [
  {
    path: 'logs',
    component: LogContainerComponent
  },
  {
    path: 'sensors',
    component: SensorsContainerComponent
  },
  {
    path: 'schedule',
    component: ScheduleContainerComponent
  },
  {
    path: 'mixingstation',
    component: MixingStationContainerComponent
  },
  {
    path: 'settings',
    component: DeviceSettingsContainerComponent
  },
  {
    path: '**',
    component: DashboardContainerComponent
  }];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
