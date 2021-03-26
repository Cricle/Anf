import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';
import { IComicApiService } from './comic-api.service.def'
import {EntityResult,ComicDetail,ChapterWithPage,ComicRef,Position, ProcessInfo, ComicSnapshot, SetResult, Bookshelf, Result, ComicEntityRef} from './model'

const part: string = "/api/v1/";
const comivPart: string = part + "Visiting";
const bookPart:string =part + 'Bookshelf';

@Injectable({
  providedIn: 'root'
})
export class ComicApiService implements IComicApiService{
  
  constructor(private http: HttpClient) {
  }
  public getComic(address:string):Observable<EntityResult<ComicEntityRef>>{
    return this.http.get<EntityResult<ComicEntityRef>>(`${comivPart}/GetComic?address=${address}`);
  }
  public search(keywork:string):Observable<EntityResult<ComicSnapshot[]>>{
    return this.http.get<EntityResult<ComicSnapshot[]>>(`${comivPart}/GetChapter?keywork=${keywork}`);
  }
  public getChapter(address:string,index:number):Observable<EntityResult<ChapterWithPage>>{
    return this.http.get<EntityResult<ChapterWithPage>>(`${comivPart}/GetChapter?address=${address}&index=${index}`);
  }
  public getEngine(address:string):Observable<EntityResult<string>>{
    return this.http.get<EntityResult<string>>(`${comivPart}/GetEngine?address=${address}`);
  }
  public makePageUrl(address:string,engineName:string):string{
    return `${comivPart}/GetPage?address=${address}&engineName=${engineName}`;
  }

  public findBookShelf(key?:string,skip?:number,take?:number):Observable<SetResult<Bookshelf>>{
    return this.http.get<SetResult<Bookshelf>>(`${bookPart}/Find?key=${key}&skip=${skip}&take=${take}`);
  }
  public removeBookShelf(address:string):Observable<Result>{
    return this.http.post<Result>(`${bookPart}/Remove`,'address='+address);
  }
  public clearBookShelf():Observable<Result>{
    return this.http.post<Result>(`${bookPart}/Clear`,null);
  }
  public addBookShelf(address:string):Observable<Result>{
    return this.http.post<Result>(`${bookPart}/Clear`,'address='+address);
  }
}
