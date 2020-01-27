import { Component, OnInit, Input } from '@angular/core';
import { Aquarium } from '../models/Aquarium';

@Component({
  selector: 'aquarium-form',
  templateUrl: './aquarium-form.component.html',
  styleUrls: ['./aquarium-form.component.scss']
})
export class AquariumFormComponent implements OnInit {
  
  public email: string;
  public password: string;
  public aquarium: Aquarium;
  @Input("disabled") disabled:boolean;
  @Input("aquariums") aquariums:Aquarium[];
  
  constructor() { }

  ngOnInit() {
  }

  public clickSelectAquarium(aq: Aquarium) {
    this.aquarium = aq;
  }

}
