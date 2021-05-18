import { Component, OnInit } from '@angular/core';
import { ComicApiService } from '../comic-api/comic-api.service';
import { AnfComicEntityTruck, ComicRankItem, SetResult } from '../comic-api/model';

import { NzNotificationService } from 'ng-zorro-antd/notification';

const coff:number=0.6;

@Component({
  selector: 'app-top-rank',
  templateUrl: './top-rank.component.html',
  styleUrls: ['./top-rank.component.css']
})
export class TopRankComponent implements OnInit {
  loading:boolean;
  rank:SetResult<ComicRankItem>;
  selectedItem:ComicRankItem;
  visitComic:AnfComicEntityTruck;
  loadingVisit:boolean;
  emptyEntity:boolean;
  showVisit:boolean;
  drawerWidth:number;

  constructor(private api: ComicApiService,
    private notify:NzNotificationService) {
    this.loading=true;
  }

  loadEntity(item:ComicRankItem){
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
  ngOnInit() {
    this.api.getTop50().subscribe(x=>{
      this.rank=x;
      console.log(x);
    },err=>{
      this.notify.error('Loading rank fail!',err);
    },()=>this.loading=false);
  }

}
