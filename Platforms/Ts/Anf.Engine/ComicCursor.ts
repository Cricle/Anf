export namespace Anf{
    export interface ComicCursor{
        keyword?:string;
        index:number;   
        take:number;    
        current?:SearchComicResult;
        moveNext():Promise<boolean>;
    }
    export interface SearchComicResult{
        support:boolean;
        snapshots:ComicSnapshot[];
        total?:number
    }
    export interface ComicSnapshot extends ComicRef{
        name:string;
        author:string;
        imageUri:string;
        sources:ComicSource[];
        descript:string;
    }
    export interface ComicSource extends ComicRef{
        name:string;
    }
    export interface ComicRef{
        targetUrl:string;
    }
    export interface ComicInfo{
        comicUrl:string;
        name:string;
        descript:string;
        imageUrl:string;
    }
    export interface ComicChapter extends ComicRef{
        title:string;
    }
    export interface ComicEntity extends ComicInfo{
        chapters:ComicChapter[];
    }
    export interface ComicPage extends ComicRef{
        name:string;
    }
    export interface ComicDetail{
        entity:ComicEntity;
        chapters:ChapterWithPage[];
    }
    export interface ChapterWithPage{
        chapter:ComicChapter;
        pages:ComicPage[];
    }
}