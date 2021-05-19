import { Component, OnInit } from '@angular/core';
import { ActivatedRoute,Router } from '@angular/router';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { ComicApiService } from '../comic-api/comic-api.service';
import { ComicSource, SearchComicResult, SetResult, SortedItem } from '../comic-api/model';

@Component({
  selector: 'app-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.css']
})
export class DetailComponent implements OnInit {

  loading:boolean;
  isError:boolean;

  keyword:string='';
  provider:string;
  searchedText:string='';
  skip?:number;
  take?:number;
  providers:string[];
  hotSearch:SetResult<SortedItem>;
  result:SearchComicResult;
  
  constructor(private route: ActivatedRoute,
    private notify:NzNotificationService,
    private api:ComicApiService,
    private rt:Router) {
    this.route.queryParamMap.subscribe(x=>{
      this.keyword=x.get("k");
      if(this.keyword){
        this.provider=x.get("p");
        const skipVal=Number.parseInt(x.get("s"));
        const takeVal=Number.parseInt(x.get("t"));
        if (!Number.isNaN(skipVal)) {
          this.skip=skipVal;
        }
        if (!Number.isNaN(takeVal)) {
          this.take=takeVal;
        }
        this.search();
      }
    });
    this.hotSearch={
      datas:[],
      take:0,
      skip:0,
      total:0,
      msg:'',
      code:0,
      succeed:true
    };
    api.getProviders().subscribe(x=>{
      this.providers=x.data;
    });
  }
  
  searchKeydown(event:KeyboardEvent){
    if (event.key=="Enter") {
      this.search();
    }
  }
  searchForce(){
    this.api.getHotSearch30().subscribe(x=>{
      this.hotSearch=x;
    });
  }
  search(){
    if (this.loading) {
      return;
    }
    if(!this.keyword){
      this.notify.warning("","Please input keyword!");
      return;
    }
    if (!this.providers||this.providers.length==0) {
      this.notify.error("Have not search provider!","The search can't run when no search provider!");
      return;
    }
    this.searchedText=this.keyword;
    this.isError=false;
    this.loading=true;
    this.result=null;
    this.api.search(this.provider,this.keyword,this.skip,this.take).subscribe(x=>{
      this.result=x.data;
    },err=>{
      this.isError=true;
      this.notify.error("The seach return fail",err);
    },()=>this.loading=false);
  }
  ngOnInit() {
  }
  goVisit(source:ComicSource){
    console.log(source);
    this.rt.navigate(['/visit',source.targetUrl]);
  }
}
