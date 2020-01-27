import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AquariumFormComponent } from './aquarium-form.component';

describe('AquariumFormComponent', () => {
  let component: AquariumFormComponent;
  let fixture: ComponentFixture<AquariumFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AquariumFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AquariumFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
