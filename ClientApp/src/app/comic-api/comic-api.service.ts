import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';
import {EntityResult,EntitySetResult,ComicChapter,ComicEntity,ComicInfo,ComicPage,ChapterWithPage,ComicRef,Position} from './model'

const part: string = "/api/v1/";
const comivPart: string = part + "visiting";

@Injectable({
  providedIn: 'root'
})
export class ComicApiService {
  constructor(private http: HttpClient) {
  }

  public getStatus():Observable<EntityResult<Position>> {
   return this.http.get<EntityResult<Position>>(`${comivPart}/GetStatus`);
  }
  public addDownload(address:string):Observable<EntityResult<ComicEntity>>{
    return this.http.get<EntityResult<ComicEntity>>(`${comivPart}/AddDownload?address=${address}`);
  }
  public getComic(address:string):Observable<EntityResult<ComicEntity>>{
    return this.http.get<EntityResult<ComicEntity>>(`${comivPart}/GetComic?address=${address}`);
  }
  public getCurrentComic():Observable<EntityResult<ComicEntity>>{
    return this.http.get<EntityResult<ComicEntity>>(`${comivPart}/GetCurrentComic`);
  }
  public getChapter(address:string,index:number):Observable<EntityResult<ChapterWithPage>>{
    return this.http.get<EntityResult<ChapterWithPage>>(`${comivPart}/GetChapterAsync?address=${address}&index=${index}`);
  }
  public getPage(address:string,chapterIndex:number,page:number):string{
    return `${comivPart}/GetPage?address=${address}&chapterIndex=${chapterIndex}&pageIndex=${page}`;
  }
}
