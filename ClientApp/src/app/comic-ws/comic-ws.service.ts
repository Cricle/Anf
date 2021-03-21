import { Inject, Injectable } from '@angular/core';

import { HubConnectionBuilder, HubConnection, HubConnectionState, LogLevel } from '@microsoft/signalr'
import { ProcessInfo } from '../comic-api/model';
import {WsComicEntityInfo,WsRemoveInfo, WsProcessChangedInfo} from './models'
const hubPath: string = '/api/hubs/comic';

const onReceivedProcessChanged:string='OnReceivedProcessChanged';
const onReceivedEntity:string='OnReceivedEntity';
const onReceivedRemoved:string='OnReceivedRemoved';
const onReceivedCleared:string='OnReceivedCleared';

@Injectable({
  providedIn: 'root'
})
export class ComicWsService {
  private hubConnection: HubConnection;

  private _wsProcessChanged: SubscribeMap<WsProcessChangedInfo>;
  private _wsRemoved: SubscribeMap<WsRemoveInfo>;
  private _wsCleared: SubscribeMap<void>;
  private _wsComicEntity: SubscribeMap<ProcessInfo>;
  constructor() {
    this._wsProcessChanged=new SubscribeMap<WsProcessChangedInfo>();
    this._wsRemoved=new SubscribeMap<WsRemoveInfo>();
    this._wsCleared=new SubscribeMap<void>();
    this._wsComicEntity=new SubscribeMap<ProcessInfo>();
  }
  public get wsProcessChanged(): SubscribeMap<WsProcessChangedInfo> {
    return this._wsProcessChanged;
  }
  public get wsRemoved(): SubscribeMap<WsRemoveInfo> {
    return this._wsRemoved;
  }
  public get wsCleared(): SubscribeMap<void> {
    return this._wsCleared;
  }
  public get wsComicEntity(): SubscribeMap<ProcessInfo> {
    return this._wsComicEntity;
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
    this.hubConnection.on(onReceivedProcessChanged, (x,y,z)=>this._wsProcessChanged.run({sign:x,current:y,total:z}));
    this.hubConnection.on(onReceivedEntity, (x)=>this._wsComicEntity.run(x));
    this.hubConnection.on(onReceivedRemoved, (x,y)=>this._wsRemoved.run({sign:x,done:y}));
    this.hubConnection.on(onReceivedCleared, ()=>this._wsCleared.run());
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
