import { Component, OnInit } from '@angular/core';
import { ClientService } from '../../services/client.service';

@Component({
  selector: 'log-page',
  templateUrl: './log-page.component.html',
  styleUrls: ['./log-page.component.scss']
})
export class LogPageComponent implements OnInit {
  deviceLog: string;
  public loading: boolean = true;

  constructor(private service: ClientService) { }

  ngOnInit() {
    this.loadDeviceLog();
  }

  loadDeviceLog() {
    this.loading = true;
    this.service.getDeviceLog().subscribe((log:string) => {
      this.deviceLog = log;
    },err => {

    },() => {
      this.loading = false;
    });
  }
}
