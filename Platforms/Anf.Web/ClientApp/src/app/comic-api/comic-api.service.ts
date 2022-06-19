import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';

import {AnfComicEntityTruck,SortedItem,RangeVisitEntity, EntityResult, RSAKeyIdentity, SearchComicResult, SetResult, WithPageChapter, RandomWordResult, WordResponse} from './model'

import * as JsEncryptModule from 'jsencrypt';


const part: string = "/api/v1/";
const rankPart:string=part+'rank';
const readingPart:string=part+'reading';
const userPart:string=part+'user';
const hitokotoPart:string=part+'word';

@Injectable({
  providedIn: 'root'
})
export class ComicApiService{

  constructor(private http: HttpClient) {
  }
  public getRandom():Observable<EntityResult<RandomWordResult>>{
    return this.http.get<EntityResult<RandomWordResult>>(`${hitokotoPart}/GetRandom`);
  }
  public getWord(wordId:number):Observable<EntityResult<WordResponse>>{
    return this.http.get<EntityResult<WordResponse>>(`${hitokotoPart}/GetWord?wordId=${wordId}`);
  }
  public getTop50():Observable<EntityResult<RangeVisitEntity>>{
    return this.http.get<EntityResult<RangeVisitEntity>>(`${rankPart}/GetRank50`);
  }
  public getChapter(url:string,entityUrl:string):Observable<WithPageChapter>{
    return this.http.get<WithPageChapter>(`${readingPart}/GetChapter?url=${url}&entityUrl=${entityUrl}`);
  }
  public getEntity(url:string):Observable<AnfComicEntityTruck>{
    return this.http.get<AnfComicEntityTruck>(`${readingPart}/GetEntity?url=${url}`);
  }
  public flushKey():Observable<EntityResult<RSAKeyIdentity>>{
    const r=Math.random();
    return this.http.get<EntityResult<RSAKeyIdentity>>(`${userPart}/FlushKey?r=${r}`);
  }
  public encrypt(publicKey:string,data:string,keyLen:string='2048'):string|false{
    const enc=new JsEncryptModule.JSEncrypt({default_key_size:keyLen});
    enc.setPublicKey(publicKey)
    const w=enc.encrypt(data);
    return w;
  }
  public login(userName:string,passwordHash:string,connectId:string):Observable<EntityResult<string>>{
    const form=new FormData();
    form.set("userName",userName);
    form.set("passwordHash",passwordHash);
    form.set("connectId",connectId);
    return this.http.post<EntityResult<string>>(`${userPart}/Login`,form);
  }
  public registe(userName:string,passwordHash:string,connectId:string):Observable<EntityResult<boolean>>{
    const form=new FormData();
    form.set("userName",userName);
    form.set("passwordHash",passwordHash);
    form.set("connectId",connectId);
    return this.http.post<EntityResult<boolean>>(`${userPart}/Registe`,form);
  }
  public getProviders():Observable<EntityResult<string[]>>{
    return this.http.get<EntityResult<string[]>>(`${readingPart}/GetProviders`);
  }
  public search(provider:string,keyword:string,skip?:number,take?:number):Observable<EntityResult<SearchComicResult>>{
    if (!skip) {
      skip=0;
    }
    if (!take) {
      take=20;
    }
    return this.http.get<EntityResult<SearchComicResult>>(`${readingPart}/search?provider=${provider}&keyword=${keyword}&skip=${skip}&take=${take}`);
  }
  public makeImgUrl(entityUrl:string,url:string):Observable<EntityResult<string>>{
    const r= `${readingPart}/GetImage?entityUrl=${entityUrl}&url=${url}`;
    return this.http.get<EntityResult<string>>(r);
  }
  public getHotSearch30():Observable<SetResult<SortedItem>>{
    return this .http.get<SetResult<SortedItem>>(`${rankPart}/GetHotSearch30`);
  }
}
