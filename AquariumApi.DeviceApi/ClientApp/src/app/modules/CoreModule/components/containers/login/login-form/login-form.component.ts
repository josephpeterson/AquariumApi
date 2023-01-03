import { Component, Input } from '@angular/core';

@Component({
  selector: 'device-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss']
})
export class LoginFormComponent {

  public email: string;
  public password: string;
  @Input() disabled:boolean;
}
