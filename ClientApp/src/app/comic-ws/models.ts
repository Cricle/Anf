import { Position } from '../comic-api/model'
export enum NotifyTypes
{
    BeginFetchPage = 0,
    Canceled = 1,
    Complated = 2,
    EndFetchPage = 3,
    FetchPageException = 4,
    NotNeedToSave = 5,
    ReadyFetch = 6,
    ReadySave = 7,
}
export interface ProcessInfo{
    type:NotifyTypes;
    name:string;
    comicUrl:string;
    chapter:ProcessItemSnapshot;
    page:ProcessItemSnapshot;
}
export interface ProcessItemSnapshot extends Position{
    name:string;
    url:string;
}