import { TestBed } from '@angular/core/testing';

import { ComicApiService } from './comic-api.service';

describe('ComicApiService', () => {
  let service: ComicApiService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ComicApiService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
