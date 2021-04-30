import { Component, OnInit } from '@angular/core';

import {ComicApiService} from '../../comic-api/comic-api.service'
import { ComicManager } from '../../comic-api/comic-mgr';
import { ComicDetail, ComicEntity, ComicRef, ComicSnapshot } from '../../comic-api/model';

@Component({
  selector: 'app-analysis-search',
  templateUrl: './analysis-search.component.html',
  styleUrls: ['./analysis-search.component.css']
})
export class AnalysisSearchComponent implements OnInit {

  comic:ComicEntity;

  constructor(private comicApi:ComicApiService,
    public mgr:ComicManager) {
  }

  ngOnInit() {
  }
  public onSearchEmit(event:KeyboardEvent){
    if (event.key=='Enter') {
      this.mgr.searchComic();
    }
  }
}
