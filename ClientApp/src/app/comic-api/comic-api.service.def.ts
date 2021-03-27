
import { HttpClient } from '@angular/common/http'
import { Observable } from 'rxjs';
import { EntityResult, ChapterWithPage, ComicRef, Position, ProcessInfo, ComicSnapshot, SetResult, Bookshelf, Result, ComicEntityRef } from './model'

export interface IComicApiService {
    getComic(address: string): Observable<EntityResult<ComicEntityRef>>;
    search(keywork: string): Observable<EntityResult<ComicSnapshot[]>>;
    getChapter(address: string, index: number): Observable<EntityResult<ChapterWithPage>>;
    getEngine(address: string): Observable<EntityResult<string>>;
    makePageUrl(address: string, engineName: string): string;

    findBookShelf(skip?: number, take?: number): Observable<SetResult<Bookshelf>>;
    removeBookShelf(address: string): Observable<Result>;
    clearBookShelf(): Observable<Result>;
    addBookShelf(address: string): Observable<Result>;
}