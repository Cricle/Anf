export interface Result {
    code: number;
    msg: string;
    succeed: boolean;
}

export interface EntityResult<T> extends Result {
    data: T;
}
export interface EntitySetResult<T> extends Result {
    data: T;
    total: number;
    skip: number | null;
    take: number | null;
}
export interface SetResult<T> extends EntitySetResult<T[]> {
}

export interface ComicInfo {
    comicUrl: string;
    name: string;
    descript: string;
    imageUrl: string;
}
export interface ComicRef {
    targetUrl: string;
}
export interface ComicChapter extends ComicRef {
    title: string;
}
export interface ComicEntity extends ComicInfo {
    chapters: ComicChapter[];
}
export interface ComicPage extends ComicRef {
    name: string;
}
export interface ChapterWithPage {
    chapter: ComicChapter;
    pages: ComicPage[];
}
export interface Position{
    current:number;
    total:number;
}
export interface ComicDetail{
    entity:ComicEntity;
    chapters:ChapterWithPage[];
}
export interface ProcessChangedInfo{
    sign:string;
    current:number;
    total:number;
}
export interface ProcessInfo extends ProcessChangedInfo{
    detail:ComicDetail;
}
export interface ComicSource extends ComicRef{
    name:string;
}
export interface ComicSnapshot extends ComicSource{
    author:string;
    imageUri:string;
    sources:ComicSource[];
    descript:string;
}