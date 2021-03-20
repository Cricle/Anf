export interface Result {
    Code: number;
    Msg: string;
    Succeed: boolean;
}

export interface EntityResult<T> extends Result {
    Data: T;
}
export interface EntitySetResult<T> extends Result {
    Data: T;
    Total: number;
    Skip: number | null;
    Take: number | null;
}
export interface SetResult<T> extends EntitySetResult<T[]> {
}

export interface ComicInfo {
    ComicUrl: string;
    Name: string;
    Descript: string;
    ImageUrl: string;
}
export interface ComicRef {
    TargetUrl: string;
}
export interface ComicChapter extends ComicRef {
    Title: string;
}
export interface ComicEntity extends ComicInfo {
    Chapters: ComicChapter[];
}
export interface ComicPage extends ComicRef {
    Name: string;
}
export interface ChapterWithPage {
    Chapter: ComicChapter;
    Pages: ComicPage[];
}
export interface Position{
    Current:number;
    Total:number;
}