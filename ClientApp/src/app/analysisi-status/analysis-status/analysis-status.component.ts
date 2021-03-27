import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router'
import { NzNotificationService } from 'ng-zorro-antd/notification';

import { ComicApiService } from '../../comic-api/comic-api.service'
import { ComicManager } from '../../comic-api/comic-mgr'
import { Bookshelf, ChapterWithPage, ComicChapter, ComicDetail, ComicEntity, ComicPage, Position, ProcessInfo } from '../../comic-api/model';

const ins:number=0.45;
const minIns:number=280;

@Component({
  selector: 'app-analysis-status',
  templateUrl: './analysis-status.component.html',
  styleUrls: ['./analysis-status.component.css']
})
export class AnalysisStatusComponent implements OnInit {
  drawerVisible:boolean;
  drawerLoading:boolean;
  drawerBookshelf:Bookshelf;
  drawerIsInBookshelf:boolean;
  drawerWith:string;
  chapterInfo:ChapterWithPage;

  constructor(private api: ComicApiService,
    private notify:NzNotificationService,
    private route:ActivatedRoute,
    public mgr:ComicManager) {
      this.drawerVisible=false;
      this.mgr.refreshBookShelf();
  }
  
  ngOnInit() {
  }

  showInfo(target:Bookshelf){
    let sceneWith=document.body.clientWidth;
    let emit=sceneWith*ins;
    emit=Math.max(minIns,emit);
    this.drawerLoading=true;
    this.drawerWith=emit+'px';
    this.drawerVisible=true;
    this.drawerBookshelf=target;
    const t=this.mgr.findBookshelf(target.comicUrl);
    this.drawerIsInBookshelf=t&&!t.append;
    this.drawerLoading=false;
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
  add(target:string){
    this.api.addBookShelf(target).subscribe(x=>{
      if (x.succeed) {
        this.notify.success('Add result','Succeed!');
        this.drawerIsInBookshelf=!this.drawerIsInBookshelf;
      }else{
        this.notify.error('Add result','Fail!');
      }
      this.mgr.refreshBookShelf();
    });
  }

  remove(target:Bookshelf){
    this.api.removeBookShelf(target.comicUrl).subscribe(x=>{
      if (x.succeed) {
        this.notify.success('Remove result','Succeed!');
        this.drawerIsInBookshelf=!this.drawerIsInBookshelf;
      }else{
        this.notify.error('Remove result','Fail!');
      }
      this.mgr.refreshBookShelf();
    });
  }
}
