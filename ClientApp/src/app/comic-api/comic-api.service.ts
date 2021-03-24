import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';
import {EntityResult,ComicDetail,EntitySetResult,ComicChapter,ComicEntity,ComicInfo,ComicPage,ChapterWithPage,ComicRef,Position, ProcessInfo, ComicSnapshot} from './model'

const part: string = "/api/v1/";
const comivPart: string = part + "visiting";

@Injectable({
  providedIn: 'root'
})
export class ComicApiService {
  
  constructor(private http: HttpClient) {
  }
  public addDownload(address:string):Observable<EntityResult<ComicDetail>>{
    return this.http.get<EntityResult<ComicDetail>>(`${comivPart}/AddDownload?address=${address}`);
  }
  public search(keywork:string):Observable<EntityResult<ComicSnapshot[]>>{
    return this.http.get<EntityResult<ComicSnapshot[]>>(`${comivPart}/Search?keywork=${keywork}`);
  }
  public getAll():Observable<EntityResult<ProcessInfo[]>>{
    return this.http.get<EntityResult<ProcessInfo[]>>(`${comivPart}/getAll`);
  }
  public getComic(address:string):Observable<EntityResult<ComicDetail>>{
    return this.http.get<EntityResult<ComicDetail>>(`${comivPart}/GetComic?address=${address}`);
  }
  public getPage(address:string,engineName:string):string{
    return `${comivPart}/GetPage?address=${address}&engineName=${engineName}`;
  }
}