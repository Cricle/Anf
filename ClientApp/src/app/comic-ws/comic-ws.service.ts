import { Inject, Injectable } from '@angular/core';
import { Observable, Subscriber } from 'rxjs'

import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr'
import { ProcessInfo } from './models';
import { ComicEntity,Position } from '../comic-api/model';

const hubPath: string = '/api/hubs/comic';

@Injectable({
  providedIn: 'root'
})
export class ComicWsService {
  private hubConnection:HubConnection;

  private infoObser: Observable<ProcessInfo>;
  private positionObser: Observable<Position>;
  private comicObser: Observable<ComicEntity>;
  private infoSub: Subscriber<ProcessInfo>;
  private prositionSub: Subscriber<Position>;
  private comicSub: Subscriber<ComicEntity>;
  constructor() {
    this.infoObser = new Observable<ProcessInfo>(x => this.infoSub = x);
    this.positionObser = new Observable<Position>(x => this.prositionSub = x);
    this.comicObser = new Observable<ComicEntity>(x => this.comicSub = x);
  }
  public get infoObservable(): Observable<ProcessInfo> {
    return this.infoObser;
  }
  public get prositionObservable(): Observable<Position> {
    return this.positionObser;
  }
  
  public get comicObservalble() : Observable<ComicEntity> {
    return this.comicObser;
  }
  
  public get status() : HubConnectionState {
    if (this.hubConnection) {
      return this.hubConnection.state;
    }
    return HubConnectionState.Disconnected;
  }
  

  public connect(): Promise<void> {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(hubPath)
      .build();
    this.hubConnection.on('OnReceivedProcessChanged', this.onReceivedProcessChanged);
    this.hubConnection.on('OnReceivedProcessInfo', this.onReceivedProcessInfo);
    this.hubConnection.on('OnReceivedEntity', this.onReceivedEntity);
    return this.hubConnection.start().catch(x => console.log(x));
  }
  public close(){
    if (this.status!=HubConnectionState.Disconnected) {
      this.hubConnection.stop();
    }
  }
  private onReceivedEntity(entity:ComicEntity){
    this.comicSub.next(entity);
  }
  private onReceivedProcessChanged(current: number, total: number) {
    const position: Position = {
      Current: current,
      Total: total
    };
    this.prositionSub.next(position);
  }
  private onReceivedProcessInfo(info: ProcessInfo) {
    this.infoSub.next(info);
  }
}
