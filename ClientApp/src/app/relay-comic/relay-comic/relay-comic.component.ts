import { Component, Input, OnInit, Output } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { ComicApiService } from '../../comic-api/comic-api.service';
import { ComicManager } from '../../comic-api/comic-mgr';
import { Bookshelf } from '../../comic-api/model';

const ins:number=0.45;
const minIns:number=280;

@Component({
  selector: 'app-relay-comic',
  templateUrl: './relay-comic.component.html',
  styleUrls: ['./relay-comic.component.css']
})
export class RelayComicComponent {
  @Input()
  @Output()
  public drawerVisible:boolean;
  @Output()
  public drawerLoading:boolean;
  @Output()
  public drawerBookshelf : Bookshelf;
  @Input()
  @Output()
  public drawerIsInBookshelf:boolean;
  @Output()
  public drawerWith:string;

  constructor(private api: ComicApiService,
    private notify:NzNotificationService,
    private route:ActivatedRoute,
    public mgr:ComicManager) {
    this.drawerVisible=false;

   }
  public showInfo(target:Bookshelf){
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

  public add(target:string){
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

  public remove(target:Bookshelf){
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
