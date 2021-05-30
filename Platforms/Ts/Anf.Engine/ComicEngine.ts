import * as AnfEng from './Engine'
type AComicSourceCondition=AnfEng.Anf.ComicSourceCondition;
type AComicSourceContext=AnfEng.Anf.ComicSourceContext;
export namespace Anf{
    export class ComicEngine extends Array<AComicSourceCondition>{
        public getComicSourceProviderType(targetUri:URL):AComicSourceCondition{
            const ctx:AComicSourceContext={
                uri:targetUri,
                source:targetUri.href
            };
            for (const eng of this) {
                const ok=eng.condition(ctx);
                if(ok){
                    return eng;
                }
            }
            return null;
        }
    }
}