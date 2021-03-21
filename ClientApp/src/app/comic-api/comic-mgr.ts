import { ComicApiService } from './comic-api.service'
import { ComicWsService } from '../comic-ws/comic-ws.service'
import { ComicDetail, ComicEntity, Position, ProcessInfo } from './model';
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ComicManager {
    private _processInfos: Map<string, ProcessInfo>;
    private _allTotal: number;
    private _allCurrent: number;

    private tk1: Object;
    private tk2: Object;
    private tk3: Object;
    private tk4: Object;

    private _isListening: boolean;


    public get processInfos(): Array<ProcessInfo> {
        if (this._processInfos) {
            const values = new Array();
            this._processInfos.forEach(x=>values.push(x));
            return values;
        }
        return [];
    }


    public get allTotal(): number {
        return this._allTotal
    }

    public get allCurrent(): number {
        return this._allCurrent;
    }
    private updatePos() {
        let t = 0;
        let c = 0;
        if (this._processInfos) {
            for (const ire of this._processInfos.values()) {
                t += ire.total;
                c += ire.current;
            }
        }
        this._allTotal = t;
        this._allCurrent = c;
    }

    constructor(private api: ComicApiService,
        private ws: ComicWsService) {
        this._processInfos=new Map<string, ProcessInfo>();
        this.listen();
        this.updateUnComplatedTasks().then(() => this.updatePos());
    }
    public listen() {
        if (this._isListening) {
            return;
        }
        this._isListening = true;
        this.tk1 = this.ws.wsCleared.subscribe(x => {
            this.updateUnComplatedTasks().then(() => this.updatePos());
        });
        this.tk2 = this.ws.wsComicEntity.subscribe(x => {
            this._processInfos.set(x.sign, x);
            this.updatePos();
        });
        this.tk3 = this.ws.wsProcessChanged.subscribe(x => {
            const entity = this._processInfos.get(x.sign);
            if (entity) {
                entity.total = x.total;
                entity.current = x.current;
                this.updatePos();
            }
        });
        this.tk4 = this.ws.wsRemoved.subscribe(x => {
            this.updatePos();
        });
    }
    public unlisten() {
        if (!this._isListening) {
            return;
        }
        this._isListening = false;
        this.ws.wsCleared.remove(this.tk1);
        this.ws.wsComicEntity.remove(this.tk2);
        this.ws.wsProcessChanged.remove(this.tk3);
        this.ws.wsRemoved.remove(this.tk4);
    }
    public async updateUnComplatedTasks(): Promise<void> {
        this._processInfos.clear();
        try {
            const res = await this.api.getAll().toPromise();
            for (const ire of res.data) {
                this._processInfos.set(ire.sign, ire);
            }
        } catch (error) {
            console.log(error);
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