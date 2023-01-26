import { Injectable } from "@angular/core";
import { NzNotificationService } from "ng-zorro-antd/notification";
import { Observable, Subscription } from "rxjs";
import { ComicApiService } from "./comic-api.service";
import { EntityResult, SearchComicResult, SetResult, SortedItem } from "./model";

const pageSize:number=20;

@Injectable({
    providedIn: 'root'
})
export class SearchService {
    loading: boolean;
    isError: boolean;

    private _keyword: string='';
    provider: string;
    searchedText: string = '';
    providers: string[]=[];
    acceptHotSearch:SortedItem[]=[];
    result: SearchComicResult;
    _currentPage:number=0;
    _totalPage:number=0;

    
    public get totalPage() : number {
      return this._totalPage;
    }
    
    
    public set totalPage(v : number) {
      this._totalPage = v;
    }
    

    public get currentPage():number{
      return this._currentPage;
    }
    
    public set currentPage(v : number) {
      this._currentPage = v;
      this.search();
    }
    

    public get keyword() : string {
      return this._keyword;
    }

    public set keyword(v : string) {
      this._keyword = v;
    }

    constructor(private api: ComicApiService,
        private notify:NzNotificationService) {
        this.flushProviders();
    }
  flushProviders():Observable<EntityResult<string[]>>{
    const ret= this.api.getProviders();
    ret.subscribe(x => {
      this.providers = x.data;
    });
    return ret;
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
      this.flushProviders().subscribe({
        next:()=>this.actualSearch(),
        error:err=>this.notify.error("Have not search provider!","The search can't run when no search provider!")
      });      
      return;
    }
    this.actualSearch();
  }
  actualSearch(){
    this.searchedText=this.keyword;
    this.isError=false;
    this.loading=true;
    this.result=null;
    const skip=Math.max(0,this.currentPage*pageSize);
    const take=pageSize;
    this.api.search(this.provider,this.keyword,skip,take).subscribe({
      next:x=>{
        this.result=x.data;
        this.totalPage=x.data.total;
        this.currentPage=Math.min(this.currentPage,this.totalPage);
      },
      error:err=>{
        this.isError=true;
        this.notify.error("The seach return fail",err);
      },
      complete:()=>this.loading=false
    });
  }
}
