import { Component, OnDestroy, OnInit } from '@angular/core';

import { ComicWsService } from '../../comic-ws/comic-ws.service'
import { ComicApiService } from '../../comic-api/comic-api.service'
import { ComicChapter, ComicDetail, ComicEntity, ComicPage, Position, ProcessInfo } from '../../comic-api/model';
import { ComicManager } from '../../comic-api/comic-mgr';

const ins:number=0.45;
const minIns:number=280;

@Component({
  selector: 'app-analysis-status',
  templateUrl: './analysis-status.component.html',
  styleUrls: ['./analysis-status.component.css']
})
export class AnalysisStatusComponent implements OnInit {
  drawerVisible:boolean;
  drawerComic:ProcessInfo;
  drawerWith:string;
  constructor(private api: ComicApiService,
    protected mgr:ComicManager) {
      this.drawerVisible=false;
      this.mgr=mgr;
  }
  
  ngOnInit() {
  }
  showInfo(target:ProcessInfo){
    let sceneWith=document.body.clientWidth;
    let emit=sceneWith*ins;
    emit=Math.max(minIns,emit);
    this.drawerWith=emit+'px';

    this.drawerComic=target;
    this.drawerVisible=true;
    console.log(target);
  }
  remove(target:ComicEntity){
    //Todo:
  }
}
