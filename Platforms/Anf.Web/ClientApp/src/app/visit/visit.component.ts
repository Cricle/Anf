import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';

import { ActivatedRoute } from '@angular/router'
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { ComicApiService } from '../comic-api/comic-api.service';
import { ComicScope, VisitManager, VisitPos } from '../comic-api/comic-visit.mgr';
import { AnfComicEntityTruck, ComicChapter, ComicPage, WithPageChapter } from '../comic-api/model';
import { BehaviorSubject, firstValueFrom, Observable, Subject } from 'rxjs'
import { CollectionViewer, DataSource } from '@angular/cdk/collections';
import { catchError, takeUntil } from 'rxjs/operators';
import { Router } from '@angular/router';

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
  totalChapterCount: number;
  comicScope: ComicScope;
  visitPos: VisitPos | null;
  visitIndex: number;
  chpPrefx:number;

  constructor(private route: ActivatedRoute,
    private router:Router,
    private api: ComicApiService,
    private notif: NzNotificationService,
    private visit: VisitManager) {
    this.visitPos = null;
    this.route.paramMap.subscribe(x => {
      this.url = x.get("url");
      this.visitIndex=Number(x.get("index"));
      if (!this.visitIndex) {
        this.visitIndex=0;
      }
      if (!this.url) {
        this.notif.error("Input error", "No url input!");
      } else {
        this.comicScope = visit.getComicScope(new URL(this.url));
        this.route.queryParamMap.subscribe(y => {
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
    this.api.getEntity(this.url).subscribe({
      next: y => {
        this.entity = y;
        this.totalChapterCount = y?.chapters.length || 0;
        if (!y) {
          this.notif.error("Can't load comic entity!", this.url);
        } else {
          this.comicScope.connect().subscribe(scopeOk => {
            if (scopeOk) {
              this.comicScope.getStatus().subscribe(pos => {
                this.visitPos = pos;
              });
            }
          });
          if (this.chapter >= 0 && this.chapter < y.chapters.length) {
            this.loadChapter(this.chapter, false);
          } else if (y.chapters.length > 0) {
            this.chapter = 0;
            this.loadChapter(0, false);
          }
          if(!this.visitIndex){
            this.visitIndex=0;
          }
          this.chpPrefx=((this.visitIndex+1)/this.entity.chapters.length)*100;
        }
      },
      error: err => {
        this.notif.error("Fail in load entity", this.url);
        this.loading = false;
      },
      complete: () => this.loading = false
    });
  }
  private token: number;
  goChapter(index:number){
    this.router.navigate(['/visit', this.url,index]);
  }
  loadChapter(index: number, record: boolean = true) {
    this.loading = true;
    this.wcp = null;
    this.watchingChapter = this.entity.chapters[index];
    this.token = Math.random();
    this.api.getChapter(this.watchingChapter.targetUrl, this.entity.comicUrl).subscribe({
      next: x => {
        this.wcp = x;
        if (x && record) {
          this.comicScope.update({
            chapter: x.targetUrl
          });
        }

        // if(x){
        //   this.goDownload(x,this.token);
        // }
      },
      error: err => {
        this.notif.error("Fail in load chapter", this.watchingChapter.targetUrl);
        this.loading = false;
      },
      complete: () => this.loading = false
    });
  }
  private goDownload(chps:WithPageChapter,currentToken:number){
    for (const chp of chps.pages) {
      if (this.token!=currentToken) {
        break;
      }
      this.api.makeImgUrl(this.entity.comicUrl,chp.targetUrl).subscribe({
        next:dt=>chp.actualUrl=dt.data
      });
    }
  }


}

