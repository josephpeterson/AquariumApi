import { Component, OnInit } from '@angular/core';
import { AquariumDeviceService } from '../../../../SharedDeviceModule/aquarium-device.service';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
@Component({
  selector: 'device-log-page',
  templateUrl: './log-page.component.html',
  styleUrls: []
})
export class LogPageComponent implements OnInit {
  public deviceLog$: Observable<string> = this.service.getDeviceLog();
  public loading = true;

  constructor(private service: AquariumDeviceService,
    private notifier: ToastrService) { }

  ngOnInit() {
    this.clickGetApplicationLog();
  }
  clickGetApplicationLog() {
    this.loading = true;
    this.deviceLog$ = this.service.getDeviceLog();

    this.deviceLog$.subscribe(data => {
      this.loading = false;
    }, err => {
      this.loading = false;
      this.notifier.error("An error occured while retrieving the application log file");
      console.log(err);
    });
  }
}
