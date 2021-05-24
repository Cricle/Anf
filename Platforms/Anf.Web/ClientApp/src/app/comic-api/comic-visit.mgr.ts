import { Injectable } from "@angular/core";

import { Observable } from 'rxjs'

const COMIC_ITEM:string="Saved_Comic";

@Injectable({
    providedIn: 'root'
})
export class VisitManager{
    constructor(){
    }
    public getComicScope(url:URL):ComicScope{
        return new LocalStorageComicScope(url);
    }
    public getSaved():Set<string>{
        const data= localStorage.getItem(COMIC_ITEM);
        if(data){
            
            return JSON.parse(data);
        }
        return new Set<string>();
    }
    public saveComic(url:URL){
        const saved=this.getSaved();
        saved.add(url.href);
        localStorage.setItem(COMIC_ITEM,JSON.stringify(saved));
    }
}
export interface ComicScope{
    getUrl() : URL;
    connect():Observable<boolean>;
    update(pos:VisitPos):Observable<boolean>;
    getStatus():Observable<VisitPos>;
    close():Observable<boolean>;
}
const COMIC_CACHE_NAME:string="COMICS";
export class LocalStorageComicScope implements ComicScope {
    private _url:URL;
    
    public getUrl() : URL {
        return this._url;
    }
    constructor(url:URL){
        if(!url){
            throw Error("The url must not null!");
        }
        this._url=url;
    }
    public close(): Observable<boolean> {
        return new Observable<boolean>(x=>{
            x.next(true);
            x.complete();
        });
    }
    public connect():Observable<boolean>{
        return new Observable<boolean>(x=>{
            x.next(true);
            x.complete();
        });
    }
    private getKey():string{
        return COMIC_CACHE_NAME+"_"+this._url;
    }
    public update(pos:VisitPos):Observable<boolean>{
        const key=this.getKey();
        return new Observable<boolean>(x=>{
            localStorage.setItem(key,JSON.stringify(pos));
            x.next(true);
            x.complete();
        });
    }
    public getStatus():Observable<VisitPos>{
        const key=this.getKey();
        return new Observable<VisitPos>(x=>{
            const data=localStorage.getItem(key);
            if(data){
                const dt:VisitPos=JSON.parse(data);
                x.next(dt);
                x.complete();
            }
        });
    }
}
export interface VisitPos{
    chapter?:string;
    pageIndex?:number;
}