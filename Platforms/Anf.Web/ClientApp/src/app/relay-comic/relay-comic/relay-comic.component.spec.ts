import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RelayComicComponent } from './relay-comic.component';

describe('RelayComicComponent', () => {
  let component: RelayComicComponent;
  let fixture: ComponentFixture<RelayComicComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RelayComicComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RelayComicComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
