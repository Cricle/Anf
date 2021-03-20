import { ComicApiService } from './comic-api.service'
import { ComicWsService } from '../comic-ws/comic-ws.service'
import { ComicEntity, Position } from './model';
import { NotifyTypes, ProcessInfo } from '../comic-ws/models';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ComicManager {
    private _unComplatedTasks: ComicEntity[];
    private _entity: ComicEntity;
    private _currentInfo: ProcessInfo;
    private _position: Position;
    private _comicPos:number;
    private _chapterPos:number;

    private tk1: Object;
    private tk2: Object;
    private tk3: Object;

    private _isListening: boolean;

    
    public get comicPos() : number {
        return this._comicPos;
    }
    
    
    public get chapterPos() : number {
        return this._chapterPos;
    }
    

    public get isListening(): boolean {
        return this._isListening;
    }

    public get currentInfo(): ProcessInfo {
        return this._currentInfo;
    }

    public get position(): Position {
        return this._position;
    }

    public get entity(): ComicEntity {
        return this._entity;
    }

    public get unComplatedTasks(): ComicEntity[] {
        if (!this._unComplatedTasks) {
            return this._unComplatedTasks;
        }
        return [];
    }

    constructor(private api: ComicApiService,
        private ws: ComicWsService) {
        this._position = {
            current: 0,
            total: 0
        };
        this._unComplatedTasks = [];
        this.listen();
        this.api.getCurrentComic().subscribe(x => {
            if (x && x.data) {
                this._entity = x.data
            }
        });
        this.updateUnComplatedTasks();
        // this.loadTemplateData();
        // this._unComplatedTasks=[this.entity,this.entity,this.entity];
        // this.updatePos();
    }
    private updatePos(){
        if (this._position) {
            this._chapterPos=(this._position.current/this._position.total)*100;
        }else{
            this._chapterPos=0;
        }
        if (this._currentInfo) {
            this._comicPos=(this._currentInfo.chapter.current/this._currentInfo.chapter.total)*100;
        }else{
            this._comicPos=0;
        }
    }
    public listen() {
        if (this._isListening) {
            return;
        }
        this._isListening = true;
        this.tk1 = this.ws.comicObservalble.subscribe(x => {
            this._entity = x;
            this.updatePos();
        });
        this.tk2 = this.ws.infoObservable.subscribe(x => {
            this._currentInfo = x;
            this.updatePos();
            console.log(x);
        });
        this.tk3 = this.ws.prositionObservable.subscribe(x => {
            this._position = x;
            this.updatePos();
        });
    }
    public unlisten() {
        if (!this._isListening) {
            return;
        }
        this._isListening = false;
        this.ws.comicObservalble.remove(this.tk1);
        this.ws.infoObservable.remove(this.tk2);
        this.ws.prositionObservable.remove(this.tk3);
    }
    public async updateUnComplatedTasks(): Promise<ComicEntity[]> {
        this._unComplatedTasks=[];
        try {
            const res = await this.api.getUnComplatedTask().toPromise();
            this._unComplatedTasks = res.data;
            return res.data;
        } catch (error) {
            return [];
        }
    }
    // private loadTemplateData() {
    //     this._entity = { "Chapters": [{ "Title": "第0话 ", "TargetUrl": "http://www.dm5.com/m684428/" }, { "Title": "第1话 魔法师の国", "TargetUrl": "http://www.dm5.com/m731467/" }, { "Title": "第2话 花の国", "TargetUrl": "http://www.dm5.com/m794202/" }, { "Title": "第3话 资金储备", "TargetUrl": "http://www.dm5.com/m841355/" }, { "Title": "第4话 魔女见习生伊蕾娜", "TargetUrl": "http://www.dm5.com/m873515/" }, { "Title": "第5话 瓶中的幸福", "TargetUrl": "http://www.dm5.com/m898858/" }, { "Title": "第6话 前篇", "TargetUrl": "http://www.dm5.com/m927472/" }, { "Title": "第7话 无民之国的王女", "TargetUrl": "http://www.dm5.com/m936254/" }, { "Title": "第8话 ", "TargetUrl": "http://www.dm5.com/m964151/" }, { "Title": "第9话 是谁在追赶逃跑的公主", "TargetUrl": "http://www.dm5.com/m997218/" }, { "Title": "第10话 在融雪之前（前篇）", "TargetUrl": "http://www.dm5.com/m1041804/" }], "ComicUrl": "http://www.dm5.com/manhua-monvzhilv/", "Name": "魔女之旅", "Descript": "魔女之旅漫画 ，只允许魔法师入境的国家、最喜欢肌肉的壮汉、在死亡深渊等待恋人归来的青年、独自留守国家早已灭亡的公主，和莫名其妙、滑稽可笑的人们相遇，接触某人美丽的日常生活，魔女日复一日编制出相逢与离别的故事。", "ImageUrl": "http://mhfm2tel.cdndm5.com/45/44726/20181130221314_360x480_65.jpg" };
    //     this._currentInfo = {
    //         name: this.entity.Name,
    //         comicUrl: this.entity.ComicUrl,
    //         chapter: {
    //             Name: this.entity.Chapters[0].Title,
    //             Url: this.entity.Chapters[0].TargetUrl,
    //             Current: 4,
    //             Total: 10
    //         },
    //         Page: {
    //             Name: "Name",
    //             Url: "http://www.bing.com",
    //             Current: this.position.Current,
    //             Total: this.position.Total
    //         },
    //         Type: NotifyTypes.BeginFetchPage
    //     };

    // }
}