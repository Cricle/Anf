

export class VisitScope {
    private _url: URL;

    public get url(): URL {
        return this._url;
    }
    constructor(url:URL){
        if(!url){
            throw Error("The url must not null!");
        }
    }
    public getCurrentStatus(){
        
    }
}