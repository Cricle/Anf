import { Inject, Injectable } from '@angular/core';
import { Observable, Subscriber } from 'rxjs'

import { HubConnectionBuilder, HubConnection, HubConnectionState, LogLevel } from '@microsoft/signalr'
import { ProcessInfo } from './models';
import { ComicEntity, Position } from '../comic-api/model';

const hubPath: string = '/api/hubs/comic';

@Injectable({
  providedIn: 'root'
})
export class ComicWsService {
  private hubConnection: HubConnection;

  private infoObser: SubscribeMap<ProcessInfo>;
  private positionObser: SubscribeMap<Position>;
  private comicObser: SubscribeMap<ComicEntity>;
  constructor() {
    this.infoObser = new SubscribeMap<ProcessInfo>();
    this.positionObser = new SubscribeMap<Position>();
    this.comicObser = new SubscribeMap<ComicEntity>();
  }
  public get infoObservable(): SubscribeMap<ProcessInfo> {
    return this.infoObser;
  }
  public get prositionObservable(): SubscribeMap<Position> {
    return this.positionObser;
  }

  public get comicObservalble(): SubscribeMap<ComicEntity> {
    return this.comicObser;
  }  

  public get status(): HubConnectionState {
    if (this.hubConnection) {
      return this.hubConnection.state;
    }
    return HubConnectionState.Disconnected;
  }


  public connect(): Promise<void> {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(hubPath)
      .configureLogging(LogLevel.Debug)
      .build();
    this.hubConnection.on('OnReceivedProcessChanged', (x,y)=>this.positionObser.run({current:x,total:y}));
    this.hubConnection.on('OnReceivedProcessInfo', (x)=>this.infoObser.run(x));
    this.hubConnection.on('OnReceivedEntity', (x)=>this.comicObser.run(x));
    return this.hubConnection.start().catch(x => console.log(x));
  }
  public close() {
    if (this.status != HubConnectionState.Disconnected) {
      this.hubConnection.stop();
    }
  }
}
type SubscribeCallBack<TValue> = (value: TValue) => void;
export class SubscribeMap<TValue>{
  private subMap: Map<Object, SubscribeCallBack<TValue>>;

  constructor(){
    this.subMap=new Map<Object, SubscribeCallBack<TValue>>();
  }

  public get length(): number {
    return this.subMap.size;
  }

  public subscribe(callback: SubscribeCallBack<TValue>): Object {
    const token = new Object();
    this.subMap.set(token, callback);
    return token;
  }
  public remove(token: Object): boolean {
    return this.subMap.delete(token);
  }
  public contains(token: Object): boolean {
    return this.subMap.has(token);
  }
  public clear(): void {
    return this.clear();
  }
  public run(val: TValue) {
    this.subMap.forEach(x => x(val));
  }
}
