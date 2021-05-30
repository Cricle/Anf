import * as AnfRes from '../ResourceBuffer'
type ABuffer = AnfRes.Anf.ResourceBuffer;
export namespace Anf.Networks {
    export interface NetworkAdapter {
        getBuffer(settings: RequestSettings): Promise<ABuffer>;
        getJson<T>(settings: RequestSettings):Promise<T>;
        getText(settings: RequestSettings):Promise<string>;
    }
    export class FetchAdapter implements NetworkAdapter {
        public getBuffer(settings: RequestSettings): Promise<ABuffer> {
            return this.getStream(settings).then(x=>{
                const buffer:ABuffer={
                    resourceBuffer:x,
                    resourceUrl:settings.address
                };
                return buffer;
            });
        }
        public getJson<T>(settings: RequestSettings): Promise<T> {
            return this.getStream(settings).then(x=>x.json());
        }
        public getText(settings: RequestSettings): Promise<string> {
            return this.getStream(settings).then(x=>x.text());
        }
        public static readonly Default: FetchAdapter = new FetchAdapter();
        
        private getStream(settings: RequestSettings): Promise<Response> {
            return new Promise<Response>(async () => {
                const init: RequestInit = {
                    method: settings.method,
                    body: settings.data,
                    referrer: settings.referrer
                };
                const headers = settings.headers || {};
                if (settings.accept) {
                    headers["Accept"] = settings.accept;
                }
                if (settings.host) {
                    headers["Host"] = settings.host;
                }
                const rep = await fetch(settings.address, init);
                return rep;
            });
        }

    }
    export interface RequestSettings {
        address: string;
        host?: string;
        method?: string;
        accept?: string;
        referrer?: string;
        timeout?: number;
        data?: any;
        headers?: Map<string, string>;
    }
}