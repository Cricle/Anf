import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ComicApiService } from '../../comic-api/comic-api.service';
import { NzNotificationService } from 'ng-zorro-antd/notification/ng-zorro-antd-notification'
import { ChapterWithPage, ComicEntity, ComicEntityRef, ComicInfo, ComicRef } from '../../comic-api/model';
@Component({
  selector: 'app-watching',
  templateUrl: './watching.component.html',
  styleUrls: ['./watching.component.css']
})
export class WatchingComponent implements OnInit {
  private _nav: ActivatedRoute;
  private _apiSer:ComicApiService;
  private _notify:NzNotificationService;
  private ref:string;
  private chapterIndef:number;
  private entityRef:ComicEntityRef;
  private currentChapter:ChapterWithPage;

  constructor(nav: ActivatedRoute,
    apiSer:ComicApiService,
    notify:NzNotificationService) {
    this._notify=notify;
    this._apiSer=apiSer;
    this._nav = nav;
    this._nav.paramMap.subscribe(x=>{
      this.ref=x.get('ref');
      this.chapterIndef=Number.parseInt(x.get('chp'));
      if (Number.isNaN(this.chapterIndef)) {
        this.chapterIndef=0;
      }
    });
  }

  ngOnInit(): void {
  }
  private loadChapter(){
    this._apiSer.getChapter(this.entityRef.entity.comicUrl,this.chapterIndef).subscribe(x=>{
      this.currentChapter=x.data;
    });
  }
  private getPageUrl(url:string):string{
    return this._apiSer.makePageUrl(url,this.entityRef.engineName);
  }
  private initReadyModel(){
    if (this.ref&&(this.ref.startsWith('http://')||this.ref.startsWith('www'))) {
      if (this.ref.startsWith('www')) {
        this.ref="http://"+this.ref;
      }
      this._apiSer.getComic(this.ref).subscribe(x=>{
        this.entityRef=x.data;
        if (this.entityRef) {
        }
      });
    }
  }
}
