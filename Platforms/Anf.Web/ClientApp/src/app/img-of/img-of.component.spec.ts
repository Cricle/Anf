import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ImgOfComponent } from './img-of.component';

describe('ImgOfComponent', () => {
  let component: ImgOfComponent;
  let fixture: ComponentFixture<ImgOfComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ImgOfComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ImgOfComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
