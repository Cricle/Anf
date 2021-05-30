import * as AnfType from './ComicCursor'
import * as AnfT from './Type'
import * as AnfRes from './ResourceBuffer'
export namespace Anf{
    export interface Engine{
        engineName:string;
    }
    export interface SearchProvider extends Engine{
        search(keyword:string,skip:number,take:number):Promise<AnfType.Anf.SearchComicResult>;
    }
    export interface ProposalProvider extends Engine{
        getProposal(take:number):Promise<AnfType.Anf.ComicSnapshot[]>;
    }
    export interface ComicSourceProvider{
        getImageStream(targetUrl:string):Promise<AnfRes.Anf.ResourceBuffer>;
        getChapters(targetUrl:string):Promise<AnfType.Anf.ComicEntity>;
        getPages(targetUrl:string):Promise<AnfType.Anf.ComicPage[]>;
    }
    export interface ProposalDescription{
        providerType:AnfT.Anf.Type<ProposalProvider>;
        name:string;
        descritionUri:URL;
    }
    export interface ComicSourceCondition extends Engine{
        order:number;
        descript?:Map<string,string>;
        ProviderType:AnfT.Anf.Type<ComicSourceProvider>;
        address:URL;
        faviconAddress:URL;
        condition(ctx:ComicSourceContext):boolean;
    }
    export interface ComicSourceContext{
        source:string;
        uri:URL;
    }
}