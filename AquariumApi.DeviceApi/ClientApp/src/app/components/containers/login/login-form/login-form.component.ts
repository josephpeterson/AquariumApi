import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss']
})
export class LoginFormComponent implements OnInit {

  public email: string;
  public password: string;
  @Input("disabled") disabled:boolean;
  
  constructor() { }

  ngOnInit() {
  }

}
