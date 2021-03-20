import { Component, OnInit } from '@angular/core';

import {ComicApiService} from '../../comic-api/comic-api.service'
import { ComicEntity } from '../../comic-api/model';
import {ComicWsService} from '../../comic-ws/comic-ws.service'

@Component({
  selector: 'app-analysis-search',
  templateUrl: './analysis-search.component.html',
  styleUrls: ['./analysis-search.component.css']
})
export class AnalysisSearchComponent implements OnInit {

  address:string='';
  searching:boolean;
  comic:ComicEntity;

  constructor(private comicApi:ComicApiService) { }

  ngOnInit() {
  }
  public onSearchEmit(event:KeyboardEvent){
    if (event.key=='Enter') {
      this.search();
    }
  }

  public search(){
    this.comic=null;
    this.searching=true;//TODO:减小传输大小
    this.comicApi.addDownload(this.address).subscribe(res=>{
      this.comic=res.Data;
    },err=>{
      console.log(err);
    },()=>this.searching=false);
  }
}
