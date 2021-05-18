import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';

import {AnfComicEntityTruck, ComicRankItem, EntityResult, RSAKeyIdentity, SetResult, WithPageChapter} from './model'

import * as JsEncryptModule from 'jsencrypt';


const part: string = "/api/v1/";
const rankPart:string=part+'rank';
const readingPart:string=part+'reading';
const userPart:string=part+'user';

@Injectable({
  providedIn: 'root'
})
export class ComicApiService{
  

  constructor(private http: HttpClient) {
    let enc=new JsEncryptModule.JSEncrypt({});
    enc.setPublicKey(`-----BEGIN PUBLIC KEY-----
    MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA5Xhq77HqGObzpBR7ajty\\nRG/iZ2SrqAS7/5OF8HBg/tCSMQSXw48MhUvWjy6lQMYu4DDnAm1GM+hBPWMLKhK2\\ngMMDJ83bc5DbEqIbAWnw//kYjPsaMuNr8f5+G//RzGPA4ZbsSdZwHYAQGGDgQ+aa\\ngjBVSUXwiRmdwEvVglenwWYwlsI53LEycorBnSsjgCDVjYWhHXoNyztztcaI/I26\\n9A973bAkFJhOpl+T2FMUGhDdG3BFRSwvYlxqsDg8rt3LV1zpZRSXLjp4ELYvp5aV\\nKV4GtwCru5RyGJrme8L3fwk8JedHkj0u9UPSPh721pyQq6QovpgbvBySlGTR+LS4\\naQIDAQAB
    -----END PUBLIC KEY-----`)
    let w=enc.encrypt("Asdfg123456");
    console.log(w);
  }
  public getTop50():Observable<SetResult<ComicRankItem>>{
    return this.http.get<SetResult<ComicRankItem>>(`${rankPart}/GetRank50`);
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
}
