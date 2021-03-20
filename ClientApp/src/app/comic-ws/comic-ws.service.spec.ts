import { TestBed } from '@angular/core/testing';

import { ComicWsService } from './comic-ws.service';

describe('ComicWsService', () => {
  let service: ComicWsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ComicWsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
