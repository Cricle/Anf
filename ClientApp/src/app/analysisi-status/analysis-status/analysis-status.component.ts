import { Component, OnInit } from '@angular/core';

import { ComicWsService } from '../../comic-ws/comic-ws.service'
import { ComicApiService } from '../../comic-api/comic-api.service'
import { ComicChapter, ComicEntity, ComicPage, Position } from '../../comic-api/model';

@Component({
  selector: 'app-analysis-status',
  templateUrl: './analysis-status.component.html',
  styleUrls: ['./analysis-status.component.css']
})
export class AnalysisStatusComponent implements OnInit {
  entity: ComicEntity;
  chapter:ComicChapter;
  maxPage:number;
  currentPage:number;
  pos:number;
  posChapter:number;
  constructor(private ws: ComicWsService,
    private api: ComicApiService) {
    this.loadTestData();
    this.chapter=this.entity.Chapters[0];
    this.maxPage=10;
    this.currentPage=2;
    this.updatePos({
      Current:50,
      Total:100
    });

    this.ws.comicObservalble.subscribe(x=>{
      this.entity=x;
    });
    this.ws.infoObservable.subscribe(x=>{
      const chapter:number=this.findIndex(this.entity.Chapters,y=>y.TargetUrl==x.ComicUrl);
      if (chapter!=-1) {
        //TODO:换成索引
      }
    });
    this.ws.prositionObservable.subscribe(x=>{

    });
  }
  private findIndex<T>(arr:T[],condition:(value:T)=>boolean):number{
    for (let i = 0; i < arr.length; i++) {
      const element = arr[i];
      if (condition(element)) {
        return i;
      }
    }
    return -1;
  }
  private updatePos(p:Position){
    this.pos=(p.Current/p.Total)*100;
    this.posChapter=70;
  }
  private loadTestData() {
    this.entity = { "Chapters": [{ "Title": "第0话 ", "TargetUrl": "http://www.dm5.com/m684428/" }, { "Title": "第1话 魔法师の国", "TargetUrl": "http://www.dm5.com/m731467/" }, { "Title": "第2话 花の国", "TargetUrl": "http://www.dm5.com/m794202/" }, { "Title": "第3话 资金储备", "TargetUrl": "http://www.dm5.com/m841355/" }, { "Title": "第4话 魔女见习生伊蕾娜", "TargetUrl": "http://www.dm5.com/m873515/" }, { "Title": "第5话 瓶中的幸福", "TargetUrl": "http://www.dm5.com/m898858/" }, { "Title": "第6话 前篇", "TargetUrl": "http://www.dm5.com/m927472/" }, { "Title": "第7话 无民之国的王女", "TargetUrl": "http://www.dm5.com/m936254/" }, { "Title": "第8话 ", "TargetUrl": "http://www.dm5.com/m964151/" }, { "Title": "第9话 是谁在追赶逃跑的公主", "TargetUrl": "http://www.dm5.com/m997218/" }, { "Title": "第10话 在融雪之前（前篇）", "TargetUrl": "http://www.dm5.com/m1041804/" }], "ComicUrl": "http://www.dm5.com/manhua-monvzhilv/", "Name": "魔女之旅", "Descript": "魔女之旅漫画 ，只允许魔法师入境的国家、最喜欢肌肉的壮汉、在死亡深渊等待恋人归来的青年、独自留守国家早已灭亡的公主，和莫名其妙、滑稽可笑的人们相遇，接触某人美丽的日常生活，魔女日复一日编制出相逢与离别的故事。", "ImageUrl": "http://mhfm2tel.cdndm5.com/45/44726/20181130221314_360x480_65.jpg" };
  }
  async ngOnInit() {
    const res = await this.api.getCurrentComic().toPromise();
    if (res && res.Data != null) {
      this.entity = res.Data;
    }
  }

}
