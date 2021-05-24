import { Component, OnInit, Input } from '@angular/core';
import { LoginInformationResponse } from '../models/LoginInformationResponse';
import { AquariumDevice } from '../models/AquariumDevice';
import { ClientService } from '../services/client.service';
import { NotifierService } from 'angular-notifier';
import { BaseException } from '../models/BaseException';
import * as moment from 'moment';

@Component({
  selector: 'exception-list',
  templateUrl: './exception-list.component.html',
  styleUrls: ['./exception-list.component.scss']
})
export class ExceptionListComponent implements OnInit {

  public exceptions: BaseException[] = [];
  loading: boolean;


  constructor(
    public service: ClientService,
    private notifier: NotifierService
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
