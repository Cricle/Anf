export interface Result {
  code?: number;
  msg?: string;
  succeed?: boolean;
}

export interface EntityResult<T> extends Result {
  data: T;
}
export interface EntitySetResult<T> extends Result {
  datas: T;
  total: number;
  skip?: number;
  take?: number;
}
export interface SetResult<T> extends EntitySetResult<T[]> {

}

export interface ComicInfo {
  comicUrl: string;
  name: string;
  descript: string;
  imageUrl: string;
}
export interface ComicRef {
  targetUrl?: string;
  actualUrl?: string;
}
export interface ComicChapter extends ComicRef {
  title: string;
}
export interface ComicEntity extends ComicInfo {
  chapters: ComicChapter[];
}
export interface ComicPage extends ComicRef {
  name?: string;
}
export interface ChapterWithPage {
  chapter: ComicChapter;
  pages: ComicPage[];
}
export interface ComicDetail {
  entity: ComicEntity;
  chapters: ChapterWithPage[];
}
export interface ComicSource extends ComicRef {
  name: string;
}
export interface ComicSnapshot extends ComicSource {
  author: string;
  imageUri: string;
  sources: ComicSource[];
  descript: string;
}
export interface AnfComicEntityInfoOnly extends ComicInfo {
  refCount: number;
  createTime: number;
  updateTime: number;
}
export interface SortedItem {
  keyword: string;
  scope: number;
}
export interface RangeVisitEntity {
  entityTrucks: AnfComicEntityTruck[];
  size: number;
  hitCount: number;
  createTime: Date
}
export interface AnfComicEntityTruck extends AnfComicEntityInfoOnly {
  chapters: ComicChapter[];
}
export interface WithPageChapterInfoOnly extends ComicChapter {
  refCount: number;
  createTime: number;
  updateTime: number;
}
export interface WithPageChapter extends WithPageChapterInfoOnly {
  pages: ComicPage[];
}
export interface RSAKeyIdentity {
  publicKey: string;
  identity: string;
}
export interface SearchComicResult {
  support: boolean;
  snapshots: ComicSnapshot[];
  total?: number;
}
export interface ComicSnapshot {
  name: string;
  author: string;
  imageUri: string;
  sources: ComicSource[];
  descript: string;
}
export interface ComicSource extends ComicRef {
  name: string;
}
export function convertCsharpDate(time: number): Date {
  const t = (time - 621356256000000000) / 10000;
  return new Date(t);
}
export interface RandomWordResult{
  words?:WordResponse[];
  hitCount:number;
  createTime:Date;
  lifeTime:string
}
export interface WordResponse{
  id:string;
  type:WordType;
  text:string;
  length:number;
  creatorId:number;
  creatorName:string;
  authorId?:number;
  authorName:string;
  likeCount:number;
  visitCount:number;
  commitTypes:CommitTypes;
  from:string;
  createTime:Date;
}
export enum CommitTypes{
  Web=0,
  Desktop=1
}
export enum WordType {
  Animation = 'a',
  Cartoon = 'b',
  Game = 'c',
  Literature = 'd',
  Original = 'e',
  Web = 'f',
  Other = 'g',
  Video = 'h',
  Poetry = 'i',
  NeteaseCloud = 'j',
  Philosophy = 'k',
  BeClever = 'l',
}
