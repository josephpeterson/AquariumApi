import { Component, OnInit } from '@angular/core';
import { AquariumDeviceService } from '../../../../../../SharedDeviceModule/aquarium-device.service';
import { BaseException } from '../../../../../../SharedDeviceModule/models/BaseException';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'device-exception-list',
  templateUrl: './exception-list.component.html',
  styleUrls: ['./exception-list.component.scss']
})
export class ExceptionListComponent implements OnInit {

  public exceptions: BaseException[] = [];
  loading: boolean;


  constructor(
    public service: AquariumDeviceService,
    private notifier: ToastrService
  ){ }

  ngOnInit() {
    this.loadExceptions();
  }

  public loadExceptions() {
    if(this.loading) return;
    this.loading = true;


    this.service.getExceptions().subscribe((exceptions:BaseException[]) => {
      this.exceptions = exceptions;
      console.log(exceptions[0]);
      this.loading = false;
    },err => {
      //todo
      this.loading = false;
    })
  }
  public any(): boolean {
    return this.exceptions.length > 0;
  }

  public readableDate(date: string) {
    return moment(date).local().calendar();
  }
}
