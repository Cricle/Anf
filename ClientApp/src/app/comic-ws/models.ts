import { Position, ProcessInfo } from '../comic-api/model'

export interface WsProcessChangedInfo extends Position{
    sign:string;
}

export interface WsRemoveInfo{
    sign:string;
    done:boolean;
}
export interface WsComicEntityInfo{
    entity:ProcessInfo;
}