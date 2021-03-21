import { Component, OnInit } from '@angular/core';

import {ComicApiService} from '../../comic-api/comic-api.service'
import { ComicDetail, ComicEntity, ComicSnapshot } from '../../comic-api/model';
import {ComicWsService} from '../../comic-ws/comic-ws.service'

@Component({
  selector: 'app-analysis-search',
  templateUrl: './analysis-search.component.html',
  styleUrls: ['./analysis-search.component.css']
})
export class AnalysisSearchComponent implements OnInit {

  address:string='';
  linkAddress:string='';
  searching:boolean;
  comic:ComicDetail;
  suggest:ComicSnapshot[];

  constructor(private comicApi:ComicApiService) {
    this.suggest=[];
  }

  ngOnInit() {
  }
  public onSearchEmit(event:KeyboardEvent){
    if (event.key=='Enter') {
      this.search();
    }
  }
  private isAddress(input:string):boolean{
    return input.startsWith('http://')||input.startsWith('wwww');
  }
  private getActualAddress(input:string):string{
    if (input.startsWith('www.')) {
      return 'http://'+input;
    }
    return input;
  }
  public async search(){
    if (this.searching) {
      return;
    }
    this.searching=true;
    const reqAddr=this.address;
    this.linkAddress=this.address;
    if (this.isAddress(reqAddr)) {
      this.comic=null;
      let addr=this.getActualAddress(reqAddr);
      try {
        const res=await this.comicApi.addDownload(addr).toPromise();
        this.comic=res.data;
      } catch (error) {
        
      }
    }else{
      this.suggest=[];
      try {
        const res=await this.comicApi.search(reqAddr).toPromise();
        this.suggest=res.data;
      } catch (error) {
      }
    }
    this.searching=false;
  }
}
