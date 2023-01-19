import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TopRankComponent } from './top-rank.component';

describe('TopRankComponent', () => {
  let component: TopRankComponent;
  let fixture: ComponentFixture<TopRankComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TopRankComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TopRankComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
