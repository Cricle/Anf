import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BookMgrComponent } from './book-mgr.component';

describe('BookMgrComponent', () => {
  let component: BookMgrComponent;
  let fixture: ComponentFixture<BookMgrComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BookMgrComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BookMgrComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
