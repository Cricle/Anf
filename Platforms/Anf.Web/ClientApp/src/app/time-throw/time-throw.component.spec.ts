import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TimeThrowComponent } from './time-throw.component';

describe('TimeThrowComponent', () => {
  let component: TimeThrowComponent;
  let fixture: ComponentFixture<TimeThrowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TimeThrowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TimeThrowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
