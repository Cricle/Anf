import { Injectable } from "@angular/core";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { ComicApiService } from "./comic-api.service";
import { SearchComicResult, SetResult, SortedItem } from "./model";



@Injectable({
    providedIn: 'root'
})
export class SearchService {
    loading: boolean;
    isError: boolean;

    private _keyword: string='';
    provider: string;
    searchedText: string = '';
    skip?: number;
    take?: number;
    providers: string[]=[];
    acceptHotSearch:SortedItem[]=[];
    hotSearch: SetResult<SortedItem>;
    result: SearchComicResult;
    
    public get keyword() : string {
      return this._keyword;
    }
    
    public set keyword(v : string) {
      this._keyword = v;
      this.flushAcceptHotSeaech();
    }
    public flushAcceptHotSeaech(){
      this.acceptHotSearch=[];
      const v=this.keyword;
      if(v&&this.hotSearch?.datas){
        if(this.hotSearch.datas.length==0){
          this.acceptHotSearch=this.hotSearch.datas;
        }else{
          const lowerV=v.toLowerCase();
          this.acceptHotSearch=this.hotSearch.datas
            .filter(x=>x.address.toLowerCase().indexOf(lowerV)!=-1);
        }
      }
    }
    
    constructor(private api: ComicApiService,
        private notify:NzNotificationService) {
        this.hotSearch = {
            datas: [],
            take: 0,
            skip: 0,
            total: 0,
            msg: '',
            code: 0,
            succeed: true
        };
        api.getProviders().subscribe(x => {
            this.providers = x.data;
        });
    }
    
  searchForce(){
    this.api.getHotSearch30().subscribe(x=>{
      this.hotSearch=x;
      this.flushAcceptHotSeaech();
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
}