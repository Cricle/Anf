export interface Result {
    code: number;
    msg: string;
    succeed: boolean;
}

export interface EntityResult<T> extends Result {
    data: T;
}
export interface EntitySetResult<T> extends Result {
    datas: T;
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
    targetUrl?: string;
}
export interface ComicChapter extends ComicRef {
    title: string;
}
export interface ComicEntity extends ComicInfo {
    chapters: ComicChapter[];
}
export interface ComicPage extends ComicRef {
    name?: string;
}
export interface ChapterWithPage {
    chapter: ComicChapter;
    pages: ComicPage[];
}
export interface ComicDetail{
    entity:ComicEntity;
    chapters:ChapterWithPage[];
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
export interface AnfComicEntityInfoOnly extends ComicInfo{
    refCount:number;
    createTime:number;
    updateTime:number;
}
export interface SortedItem{
    address:string;
    scope:number;
}
export interface AnfComicEntityTruck extends AnfComicEntityInfoOnly{
    chapters:ComicChapter[];
}
export interface WithPageChapterInfoOnly extends ComicChapter{
    refCount:number;
    createTime:number;
    updateTime:number;
}
export interface WithPageChapter extends WithPageChapterInfoOnly{
    pages:ComicPage[];
}
export interface RSAKeyIdentity{
    key:string;
    identity:string;
}
export interface SearchComicResult{
    support:boolean;
    snapshots:ComicSnapshot[];
    total?:number;
}
export interface ComicSnapshot{
    name:string;
    author:string;
    imageUri:string;
    sources:ComicSource[];
    descript:string;
}
export interface ComicSource extends ComicRef{
    name:string;
}
export function convertCsharpDate(time:number):Date{
    const t=(time-621356256000000000)/10000;
    return new Date(t);
}