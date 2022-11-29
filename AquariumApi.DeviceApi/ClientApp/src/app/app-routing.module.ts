import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LogPageComponent as LogContainerComponent } from './components/containers/logs/log-page.component';
import { DashboardContainerComponent } from './components/containers/dashboard/dashboard-container.component';
import { LoginContainerComponent } from './components/containers/login/login-container.component';
import { MixingStationContainer } from './components/containers/mixing-station/mixing-station-container.component';

const routes: Routes = [
  {
    path: 'logs',
    component: LogContainerComponent
  },
  {
    path: 'mixingstation',
    component: MixingStationContainer
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
