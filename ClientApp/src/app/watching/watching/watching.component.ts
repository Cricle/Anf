import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ComicApiService } from '../../comic-api/comic-api.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';

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
  
  chapterIndef:number;
  entityRef:ComicEntityRef;
  currentChapter:ChapterWithPage;

  constructor(nav: ActivatedRoute,
    apiSer:ComicApiService,
    notify:NzNotificationService) {
    this._notify=notify;
    this._apiSer=apiSer;
    this._nav = nav;
    this._nav.queryParamMap.subscribe(x=>{
      this.ref=x.get('ref');
      this.chapterIndef=Number.parseInt(x.get('chp'));
      if (Number.isNaN(this.chapterIndef)) {
        this.chapterIndef=0;
      }
      this.initReadyModel();
    });
  }
  private updateTitle(){
    //this._title.setTitle(this.entityRef.entity.name+' - '+this.currentChapter.chapter.title);
  }

  ngOnInit(): void {
  }
  private loadChapter(){
    this._apiSer.getChapter(this.entityRef.entity.comicUrl,this.chapterIndef).subscribe(x=>{
      this.currentChapter=x.data;
      this.updateTitle();
    });
  }
  getPageUrl(url:string):string{
    return this._apiSer.makePageUrl(url,this.entityRef.engineName);
  }
  goChapter(index:number){
    this.chapterIndef=index;
    this.loadChapter();
  }
  goBack(){
    history.back();
  }
  private initReadyModel(){
    if (this.ref&&(this.ref.startsWith('http://')||this.ref.startsWith('https://')||this.ref.startsWith('www'))) {
      if (this.ref.startsWith('www')) {
        this.ref="http://"+this.ref;
      }
      this._apiSer.getComic(this.ref).subscribe(x=>{
        this.entityRef=x.data;
        if (this.entityRef) {
          this.loadChapter();
        }
      });
    }
  }
}
