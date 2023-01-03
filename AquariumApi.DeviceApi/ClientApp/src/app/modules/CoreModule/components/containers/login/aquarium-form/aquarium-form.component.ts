import { Component, Input } from '@angular/core';
import { Aquarium } from 'src/app/models/Aquarium';

@Component({
  selector: 'device-aquarium-form',
  templateUrl: './aquarium-form.component.html',
  styleUrls: ['./aquarium-form.component.scss']
})
export class AquariumFormComponent {
  
  public email: string;
  public password: string;
  public aquarium: Aquarium;
  @Input() disabled:boolean;
  @Input() aquariums:Aquarium[];
  public clickSelectAquarium(aq: Aquarium) {
    this.aquarium = aq;
  }

}
