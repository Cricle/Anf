import { Component, OnInit } from '@angular/core';
import { NzNotificationService } from 'ng-zorro-antd/notification';

import { VisitManager } from '../comic-api/comic-visit.mgr'
import { SortedItem,SetResult } from '../comic-api/model';
import { UserManager } from '../comic-api/usermanager';

@Component({
  selector: 'app-bookshelf',
  templateUrl: './bookshelf.component.html',
  styleUrls: ['./bookshelf.component.css']
})
export class BookshelfComponent implements OnInit {

  loading:boolean;
  userMgr:UserManager;
  rank: SetResult<SortedItem>;
  constructor(private visitMgr: VisitManager,
    userMgr:UserManager,
    private notify:NzNotificationService) {
      this.flushSaved();
      this.userMgr=userMgr;
  }
  flushSaved(){
    this.loading=true;
    try{
      const set=this.visitMgr.getSaved();
      const datas:SortedItem[]=[];
      for (const iterator of set) {
        datas.push({
          keyword:iterator,
          scope:-1
        });
      }
      this.rank={
        skip:0,
        take:set.size,
        total:set.size,
        datas:datas
      };
    }catch(err){
      this.notify.error("Load bookshelf fail!",err);
    }
  }

  ngOnInit(): void {
  }

}
