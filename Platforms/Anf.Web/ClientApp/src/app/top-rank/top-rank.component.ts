import { Component, OnInit } from '@angular/core';
import { ComicApiService } from '../comic-api/comic-api.service';
import { AnfComicEntityTruck,SortedItem, SetResult, SearchComicResult, EntityResult, RangeVisitEntity } from '../comic-api/model';
import { NzNotificationService } from 'ng-zorro-antd/notification';



@Component({
  selector: 'app-top-rank',
  templateUrl: './top-rank.component.html',
  styleUrls: ['./top-rank.component.css']
})
export class TopRankComponent implements OnInit {
  loading:boolean;
  rank: EntityResult<RangeVisitEntity>;
  constructor(private api: ComicApiService,
    private notify: NzNotificationService) {

  }

  ngOnInit() {
    this.flushRank(false);
  }
  flushRank(notify:boolean=false){
    this.api.getTop50().subscribe({
      next:x=>{
        this.rank=x;
        if(notify){
          this.notify.success('Flush hot rank succeed','Alreadly flush the relay hot rank!');
        }
      },
      error:err=>{
        this.notify.error('Loading rank fail!',err);
      },
      complete:()=>this.loading=false
    });
  }
}
