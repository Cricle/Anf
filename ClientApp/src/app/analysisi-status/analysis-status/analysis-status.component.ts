import { Component, OnDestroy, OnInit } from '@angular/core';

import { ComicWsService } from '../../comic-ws/comic-ws.service'
import { ComicApiService } from '../../comic-api/comic-api.service'
import { ComicChapter, ComicEntity, ComicPage, Position } from '../../comic-api/model';
import { NotifyTypes, ProcessInfo } from '../../comic-ws/models';
import { ComicManager } from '../../comic-api/comic-mgr';

@Component({
  selector: 'app-analysis-status',
  templateUrl: './analysis-status.component.html',
  styleUrls: ['./analysis-status.component.css']
})
export class AnalysisStatusComponent implements OnInit {
  drawerVisible:boolean;
  drawerComic:ComicEntity;
  constructor(private api: ComicApiService,
    protected mgr:ComicManager) {
      this.drawerVisible=false;
      this.mgr=mgr;
  }
  
  ngOnInit() {
  }
  showInfo(target:ComicEntity){
    this.drawerComic=target;
    this.drawerVisible=true;
    console.log(target);
  }
  remove(target:ComicEntity){
    //Todo:
  }
}
