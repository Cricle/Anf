import { Component, OnInit } from '@angular/core';
import { NzNotificationService } from 'ng-zorro-antd/notification/ng-zorro-antd-notification'
import { ActivatedRoute } from '@angular/router'

import { ComicApiService } from '../../comic-api/comic-api.service'
import { Bookshelf, ComicChapter, ComicDetail, ComicEntity, ComicPage, Position, ProcessInfo } from '../../comic-api/model';

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
  drawerComic:ComicEntity;
  drawerBookshelf:Bookshelf;
  drawerIsInBookshelf:boolean;
  drawerWith:string;

  total:number;
  bookshelfs:Bookshelf[];
  key:string;

  constructor(private api: ComicApiService,
    private notify:NzNotificationService,
    private route:ActivatedRoute) {
      this.drawerVisible=false;
      this.bookshelfs=[];
  }
  
  ngOnInit() {
  }

  public updateBookshelf(){
    this.api.findBookShelf(this.key).subscribe(x=>{
      this.bookshelfs=x.data;
      this.total=x.total;
    });
  }

  async showInfo(target:Bookshelf){
    let sceneWith=document.body.clientWidth;
    let emit=sceneWith*ins;
    emit=Math.max(minIns,emit);
    this.drawerLoading=true;
    this.drawerWith=emit+'px';
    this.drawerBookshelf=target;
    this.drawerIsInBookshelf=this.bookshelfs&&this.bookshelfs.filter(x=>x.comicUrl==target.comicUrl).length!=0;
    try {
      this.drawerVisible=true;
      await this.api.getComic(target.comicUrl).toPromise();
    } catch (error) {
      this.notify.error('Load fail!',error);
    }
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
  add(target:string){
    this.api.addBookShelf(target).subscribe(x=>{
      if (x.succeed) {
        this.notify.success('Add result','Succeed!');
      }else{
        this.notify.error('Add result','Fail!');
      }
    });
  }

  remove(target:Bookshelf){
    this.api.removeBookShelf(target.comicUrl).subscribe(x=>{
      if (x.succeed) {
        this.notify.success('Remove result','Succeed!');
      }else{
        this.notify.error('Remove result','Fail!');
      }
    });
  }
}
