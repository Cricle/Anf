import { Component, OnInit } from '@angular/core';
import { ComicApiService } from '../comic-api/comic-api.service';
import { AnfComicEntityTruck,SortedItem, SetResult, SearchComicResult } from '../comic-api/model';

import { NzNotificationService } from 'ng-zorro-antd/notification';

const coff:number=0.6;

@Component({
  selector: 'app-top-rank',
  templateUrl: './top-rank.component.html',
  styleUrls: ['./top-rank.component.css']
})
export class TopRankComponent implements OnInit {
  loading:boolean;
  rank:SetResult<SortedItem>;
  selectedItem:SortedItem;
  visitComic:AnfComicEntityTruck;
  loadingVisit:boolean;
  emptyEntity:boolean;
  showVisit:boolean;
  drawerWidth:number;

  constructor(private api: ComicApiService,
    private notify:NzNotificationService) {
    this.loading=true;
    
  }

  loadEntity(item:SortedItem){
    const locItem=item;
    this.selectedItem=item;
    this.showVisit=true;
    this.loadingVisit=true;
    this.emptyEntity=false;
    this.visitComic=null;
    this.drawerWidth=Math.max(300,document.body.clientWidth*coff);
    this.api.getEntity(item.address).subscribe(x=>{
      if(this.selectedItem==locItem){
        this.visitComic = x;
        this.emptyEntity=x==null;
        console.log(x);
      }
    },err=>{
      this.notify.error('Loading visit entity fail!',err);
    },()=>this.loadingVisit=false);
  }
  close(){
    this.showVisit=false;
  }
  copy(event:MouseEvent){
  }
  flushRank(notify:boolean=false){
    this.api.getTop50().subscribe(x=>{
      this.rank=x;
      if(notify){
        this.notify.success('Flush hot rank succeed','Alreadly flush the relay hot rank!');
      }
    },err=>{
      this.notify.error('Loading rank fail!',err);
    },()=>this.loading=false);
  }
  ngOnInit() {
    this.flushRank(false);
  }

}
