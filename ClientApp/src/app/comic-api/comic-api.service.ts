import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';
import { IComicApiService } from './comic-api.service.def'
import {EntityResult,ComicDetail,ChapterWithPage,ComicRef,Position, ProcessInfo, ComicSnapshot, SetResult, Bookshelf, Result, ComicEntityRef} from './model'

const part: string = "https://www.bing.com/api/v1/";
const comivPart: string = part + "Visiting";
const bookPart:string =part + 'Bookshelf';

@Injectable({
  providedIn: 'root'
})
export class ComicApiService implements IComicApiService{
  
  constructor(private http: HttpClient) {
  }
  public getComic(address:string):Observable<EntityResult<ComicEntityRef>>{
    return this.http.get<EntityResult<ComicEntityRef>>(`${comivPart}/GetComic?address=${encodeURIComponent(address)}`);
  }
  public search(keywork:string):Observable<EntityResult<ComicSnapshot[]>>{
    return this.http.get<EntityResult<ComicSnapshot[]>>(`${comivPart}/Search?keywork=${keywork}`);
  }
  public getChapter(address:string,index:number):Observable<EntityResult<ChapterWithPage>>{
    return this.http.get<EntityResult<ChapterWithPage>>(`${comivPart}/GetChapter?address=${encodeURIComponent(address)}&index=${index}`);
  }
  public getEngine(address:string):Observable<EntityResult<string>>{
    return this.http.get<EntityResult<string>>(`${comivPart}/GetEngine?address=${encodeURIComponent(address)}`);
  }
  public makePageUrl(address:string,engineName:string):string{
    return `${comivPart}/GetPage?address=${encodeURIComponent(address)}&engineName=${engineName}`;
  }

  public findBookShelf(skip?:number,take?:number):Observable<SetResult<Bookshelf>>{
    let queryStr='';
    if (skip) {
      queryStr='skip='+skip;
    }
    if (take) {
      let str='take='+take;
      if (queryStr='') {
        queryStr=str;
      }else{
        queryStr=queryStr+'&'+str;
      }
    }
    return this.http.get<SetResult<Bookshelf>>(`${bookPart}/Find?`+queryStr);
  }
  public updateIndex(address:string,chapterIndex:number,pageIndex?:number):Observable<Result>{
    let queryStr='chapterIndex='+chapterIndex;
    if (pageIndex) {
      queryStr=queryStr+'&pageIndex='+pageIndex;
    }
    return this.http.get<Result>(`${bookPart}/UpdateIndex?address=${encodeURIComponent(address)}?${queryStr}`);
  }
  public removeBookShelf(address:string):Observable<Result>{
    const form=new FormData();
    form.append('address',address);
    return this.http.post<Result>(`${bookPart}/Remove`,form);
  }
  public clearBookShelf():Observable<Result>{
    return this.http.post<Result>(`${bookPart}/Clear`,null);
  }
  public addBookShelf(address:string):Observable<Result>{
    const form=new FormData();
    form.append('address',address);
    return this.http.post<Result>(`${bookPart}/Add`,form);
  }
}
