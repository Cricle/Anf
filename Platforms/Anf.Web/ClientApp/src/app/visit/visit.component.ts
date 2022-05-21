import { Component, OnInit } from '@angular/core';

import { ActivatedRoute } from '@angular/router'
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { ComicApiService } from '../comic-api/comic-api.service';
import { ComicScope, VisitManager, VisitPos } from '../comic-api/comic-visit.mgr';
import { AnfComicEntityTruck, ComicChapter, WithPageChapter } from '../comic-api/model';

@Component({
  selector: 'app-visit',
  templateUrl: './visit.component.html',
  styleUrls: ['./visit.component.css']
})
export class VisitComponent implements OnInit {

  loading: boolean;
  url: string;
  chapter: number;
  entity: AnfComicEntityTruck;
  watchingChapter: ComicChapter;
  wcp: WithPageChapter;
  totalChapterCount:number;
  comicScope:ComicScope;
  visitPos:VisitPos|null;
  visitIndex:number|null;
  constructor(private route: ActivatedRoute,
    private api: ComicApiService,
    private notif: NzNotificationService,
    private visit:VisitManager) {
      this.visitPos=null;
    this.route.paramMap.subscribe(x => {
      this.url = x.get("url");
      console.log(x);
      if (!this.url) {
        this.notif.error("Input error", "No url input!");
      } else {
        this.comicScope=visit.getComicScope(new URL(this.url));
        this.route.queryParamMap.subscribe(y=>{
          const chapterStr = Number.parseInt(y.get("c"));
          if (!Number.isNaN(chapterStr)) {
            this.chapter = chapterStr;
          }
          this.loadEntity();
        });
      }
    });
  }

  ngOnInit(): void {
  }
  loadEntity() {
    this.loading = true;
    this.entity = null;
    this.api.getEntity(this.url).subscribe(y => {
      this.entity = y;
      this.totalChapterCount=y?.chapters.length||0;
      console.log(this.totalChapterCount);
      if (!y) {
        this.notif.error("Can't load comic entity!", this.url);
      } else {
        this.comicScope.connect().subscribe(scopeOk=>{
          if(scopeOk){
            this.comicScope.getStatus().subscribe(pos=>{
              this.visitPos=pos;
              this.visitIndex=this.entity.chapters.findIndex(c=>c.targetUrl==this.visitPos.chapter);
              console.log(pos,this.chapter,this.visitIndex);
            });
          }
        });
        if (this.chapter >= 0 && this.chapter < y.chapters.length) {
          this.loadChapter(this.chapter,false);
        } else if (y.chapters.length > 0) {
          this.chapter=0;
          this.loadChapter(0,false);
        }
      }
    }, err => {
      this.notif.error("Fail in load entity", this.url);
      this.loading=false;
    }, () => this.loading = false);
  }
  loadChapter(index: number,record:boolean=true) {
    this.loading = true;
    this.wcp = null;
    this.watchingChapter = this.entity.chapters[index];
    this.api.getChapter(this.watchingChapter.targetUrl, this.entity.comicUrl).subscribe(x => {
      this.wcp = x;
      if(x&&record){
        this.comicScope.update({
          chapter:x.targetUrl
        }).subscribe();
      }
    }, err => {
      this.notif.error("Fail in load chapter", this.watchingChapter.targetUrl);
      this.loading=false;
    }, () => this.loading = false);
  }
  createImgUrl(url:string):string{
    return this.api.makeImgUrl(this.entity.comicUrl,url);
  }
}
