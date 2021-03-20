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