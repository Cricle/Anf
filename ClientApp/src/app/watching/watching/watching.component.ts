import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ComicApiService } from '../../comic-api/comic-api.service';
import { ComicManager } from '../../comic-api/comic-mgr';
import { NzNotificationService } from 'ng-zorro-antd/notification/ng-zorro-antd-notification'
import { ComicEntity } from '../../comic-api/model';
@Component({
  selector: 'app-watching',
  templateUrl: './watching.component.html',
  styleUrls: ['./watching.component.css']
})
export class WatchingComponent implements OnInit {
  private _nav: ActivatedRoute;
  private _apiSer:ComicApiService;
  private _comicMgr:ComicManager;
  private _notify:NzNotificationService;
  private ref:string;

  constructor(nav: ActivatedRoute,
    apiSer:ComicApiService,
    comicMgr:ComicManager,
    notify:NzNotificationService) {
    this._notify=notify;
    this._comicMgr=comicMgr;
    this._apiSer=apiSer;
    this._nav = nav;
    this._nav.paramMap.subscribe(x=>{
      this.ref=x.get('ref');
    });
  }

  ngOnInit(): void {
  }

  private initReadyModel(){
    if (this.ref&&(this.ref.startsWith('http://')||this.ref.startsWith('www'))) {
      if (this.ref.startsWith('www')) {
        this.ref="http://"+this.ref;
        const ds=this._comicMgr.processInfos.filter(x=>x.detail.entity.comicUrl==this.ref);
        if (ds.length!=0) {
        }else{
          this._apiSer.addDownload(this.ref).subscribe(x=>{
            //TODO:这里也改成返回ProcessInfo
            const title="Add result";
            const text="Add download "+(x.succeed?"succeed":"fail");
            if (x.succeed) {
              this._notify.success(title,text);
              this.initReadyModel();
            }else{
              this._notify.error(title,text); 
            }
          });
        }
      }
    }
  }
}
