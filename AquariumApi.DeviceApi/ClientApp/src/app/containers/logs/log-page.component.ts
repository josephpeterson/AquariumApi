import { Component, OnInit } from '@angular/core';
import { ClientService } from '../../services/client.service';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'log-page',
  templateUrl: './log-page.component.html',
  styleUrls: ['./log-page.component.scss']
})
export class LogPageComponent implements OnInit {
  deviceLog$ = this.service.getDeviceLog();
  public loading: boolean = true;
  refreshing: boolean;

  constructor(private service: ClientService,
    private notifier: NotifierService) { }

  ngOnInit() {
  }
  clickGetApplicationLog() {
    this.refreshing = true;
    this.deviceLog$ = this.service.getDeviceLog();

    this.deviceLog$.subscribe(data => {
      this.refreshing = false;
    }, err => {
      this.refreshing = false;
      this.notifier.notify("error", "An error occured while retrieving the application log file");
      console.log(err);
    });
  }
}
