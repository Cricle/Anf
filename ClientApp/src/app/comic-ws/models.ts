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
    Type:NotifyTypes;
    Name:string;
    ComicUrl:string;
    Chapter:ProcessItemSnapshot;
    Page:ProcessItemSnapshot;
}
export interface ProcessItemSnapshot{
    Name:string;
    Url:string;
}