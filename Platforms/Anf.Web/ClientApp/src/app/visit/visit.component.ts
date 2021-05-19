import { Component, OnInit } from '@angular/core';

import { ActivatedRoute } from '@angular/router'
import { NzNotificationService } from '_ng-zorro-antd@11.4.1@ng-zorro-antd/notification';
import { ComicApiService } from '../comic-api/comic-api.service';
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
  constructor(private route: ActivatedRoute,
    private api: ComicApiService,
    private notif: NzNotificationService) {
    this.route.paramMap.subscribe(x => {
      this.url = x.get("url");
      console.log(x);
      if (!this.url) {
        this.notif.error("Input error", "No url input!");
      } else {
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
      if (!y) {
        this.notif.error("Can't load comic entity!", this.url);
      } else {
        if (this.chapter >= 0 && this.chapter < y.chapters.length) {
          this.loadChapter(this.chapter);
        } else if (y.chapters.length > 0) {
          this.loadChapter(0);
        }
      }
    }, err => {
      this.notif.error("Fail in load entity", this.url);
    }, () => this.loading = false);
  }
  loadChapter(index: number) {
    this.loading = true;
    this.wcp = null;
    this.watchingChapter = this.entity.chapters[index];
    this.api.getChapter(this.watchingChapter.targetUrl, this.entity.comicUrl).subscribe(x => {
      this.wcp = x;
    }, err => {
      this.notif.error("Fail in load chapter", this.watchingChapter.targetUrl);
    }, () => this.loading = false);
  }
}
