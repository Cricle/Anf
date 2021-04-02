import { NzNotificationService } from 'ng-zorro-antd/notification';
import { Observable } from 'rxjs'
import { Injectable } from '_@angular_core@8.2.12@@angular/core';

import { ComicApiService } from "./comic-api.service";
import { Bookshelf, BookshelfInfo, ComicSnapshot, SetResult } from "./model";

@Injectable({
    providedIn: 'root'
})
export class ComicManager {

    private _bookshelfs: BookshelfInfo[];
    private _snapshots: ComicSnapshot[];
    private _filtedBookshelfs: BookshelfInfo[];
    private _keyword: string;
    private _searching: boolean;

    public get searching(): boolean {
        return this._searching;
    }

    public get keyword(): string {
        return this._keyword;
    }
    public set keyword(v: string) {
        this._keyword = v;
        if (this._keyword == '') {
            this._snapshots = [];
        }
    }

    public get filtedBookshelfs(): BookshelfInfo[] {
        return this._filtedBookshelfs;
    }


    public get snapshots(): ComicSnapshot[] {
        return this._snapshots;
    }


    public get bookshelfs(): BookshelfInfo[] {
        return this._bookshelfs;
    }

    constructor(private _api: ComicApiService,
        private _notify: NzNotificationService) {
    }

    public findBookshelf(address: string): BookshelfInfo {
        for (let i = 0; i < this._bookshelfs.length; i++) {
            const element = this._bookshelfs[i];
            if (element.bookshelf.comicUrl == address) {
                return element;
            }
        }
        return null;
    }

    public refreshBookShelf(): Observable<SetResult<Bookshelf>> {
        this._filtedBookshelfs = [];
        this._bookshelfs = [];
        const obser = this._api.findBookShelf();
        obser.subscribe(x => {
            if (x.datas) {
                this._bookshelfs = x.datas.map(x => {
                    return {
                        bookshelf: x,
                        append: false
                    };
                });
            }
            this.filterBookShelf();
        });
        return obser;
    }
    private isAddress(input: string): boolean {
        return input.startsWith('http://') || input.startsWith('https://') || input.startsWith('www.');
    }
    private getActualAddress(input: string): string {
        if (input.startsWith('www.')) {
            return 'http://' + input;
        }
        return input;
    }
    public async searchComic() {
        if (this.searching) {
            return;
        }
        this._searching = true;
        const key = this._keyword;
        try {
            if (this.isAddress(key)) {
                const actualUrl = this.getActualAddress(key);
                const val = this.bookshelfs.filter(x => x.bookshelf.comicUrl == actualUrl);
                if (val.length != 0) {
                    this._notify.info('It has in your bookshelf!', 'The ' + actualUrl);
                    return;
                }
                const res = await this._api.getComic(actualUrl).toPromise();
                if (res.data) {

                    const item: BookshelfInfo = {
                        bookshelf: {
                            entity: res.data.entity,
                            comicUrl: res.data.entity.comicUrl,
                            readChapter: 0,
                            readPage: 0,
                            createTime: 0
                        },
                        append: true
                    };
                    this.bookshelfs.push(item);
                    this.filterBookShelf();
                }
            } else {
                const res = await this._api.search(key).toPromise();
                this._snapshots = res.data;
            }
        } catch (error) {
            let errMsg = error;
            if (typeof errMsg != 'string') {
                errMsg = '';
                for (const item in error) {
                    errMsg += `${item} = ${error[item]},`;
                }
            }
            this._notify.error(`Search or parse ${key} fail!`, error);
        }
        this._searching = false;
    }
    public filterBookShelf(): BookshelfInfo[] {
        if (this._keyword && this._keyword != '') {
            return this._filtedBookshelfs = this._bookshelfs.filter(x => x.bookshelf.entity.name.indexOf(this._keyword) != -1 || x.bookshelf.entity.descript.indexOf(this._keyword) != -1);
        }
        return this._filtedBookshelfs = this.bookshelfs;
    }
}