import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LogPageComponent as LogContainerComponent } from './containers/logs/log-page.component';
import { DashboardContainerComponent } from './containers/dashboard/dashboard-container.component';
import { LoginContainerComponent } from './containers/login/login-container.component';

const routes: Routes = [
  {
    path: 'logs',
    component: LogContainerComponent
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
