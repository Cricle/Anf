import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router'
import { NzNotificationService } from 'ng-zorro-antd/notification';

import { ComicApiService } from '../../comic-api/comic-api.service'
import { ComicManager } from '../../comic-api/comic-mgr'
import { Bookshelf, ChapterWithPage, ComicChapter, ComicDetail, ComicEntity, ComicPage, Position, ProcessInfo } from '../../comic-api/model';


@Component({
  selector: 'app-analysis-status',
  templateUrl: './analysis-status.component.html',
  styleUrls: ['./analysis-status.component.css']
})
export class AnalysisStatusComponent implements OnInit {
  chapterInfo:ChapterWithPage;

  drawerIsInBookshelf:boolean;
  drawerBookshelf:Bookshelf;

  constructor(private api: ComicApiService,
    private notify:NzNotificationService,
    public mgr:ComicManager) {
      this.mgr.refreshBookShelf();
  }
  
  ngOnInit() {
  }

  
  clear(){
    this.api.clearBookShelf().subscribe(x=>{
      if (x.succeed) {
        this.notify.success('Clear result','Succeed!');
      }else{
        this.notify.error('Clear result','Fail!');
      }
    });
  }
  goChapter(){

  }
}
