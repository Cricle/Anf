import { Component, OnDestroy, OnInit } from '@angular/core';
import { ComicApiService } from 'src/app/comic-api/comic-api.service';
import { RandomWordResult } from 'src/app/comic-api/model';

@Component({
  selector: 'app-giant-screen',
  templateUrl: './giant-screen.component.html',
  styleUrls: ['./giant-screen.component.css']
})
export class GiantScreenComponent implements OnInit,OnDestroy {
  waitTime:number=20000;
  timeInterval:number=1000;
  result:RandomWordResult;
  timeOut:number=0;
  timeStep:number=0;
  timePre:number=0;
  constructor(private service:ComicApiService) { }
  ngOnDestroy(): void {
    if (this.timeOut) {
      clearInterval(this.timeOut);
    }
  }

  ngOnInit() {
    let timerHandler:TimerHandler=()=>this.flushHello(this);
    this.timeOut=setInterval(timerHandler,this.timeInterval);
    this.coreFlushHello(this);
  }

  coreFlushHello(component:GiantScreenComponent){
    component.service.getRandom().subscribe(x=>{
      component.result=x.data;
    });
  }

  flushHello(component:GiantScreenComponent){
    if(component.timeStep>=component.waitTime){
      component.coreFlushHello(component);
      component.timeStep=0;
      component.timePre=0;
    }else{
      component.timeStep+=component.timeInterval;
      component.timePre=(component.timeStep/component.waitTime)*100;
    }
  }
}
