import * as AnfType from './Type'
import * as AnfResult from './ComicCursor'
import * as AnfEngine from './Engine'

type SResult = AnfResult.Anf.SearchComicResult;
export namespace Anf {
    export class SearchEngine extends Array<AnfType.Anf.Type<AnfEngine.Anf.SearchProvider>>{
        public getSearchCursorAsync(keyword: string, skip: number = 0, take: number = 0): AnfResult.Anf.ComicCursor {
            return new DefaultComicCursor(this, keyword);
        }
        public search(keyword: string, skip: number, take: number): Promise<AnfResult.Anf.SearchComicResult> {
            return new Promise(async () => {
                const result: SResult = {
                    support: false,
                    snapshots: [],
                    total: 0
                };
                for (const eng of this) {
                    const provider = eng.create();
                    const providerResult = await provider.search(keyword, skip, take);
                    if (providerResult && providerResult.snapshots && providerResult.support) {
                        result.support = true;
                        for (const sn of providerResult.snapshots) {
                            result.snapshots.push(sn);
                        }
                        result.total += providerResult.total;
                    }
                }
                return result;
            });
        }
    }
    class DefaultComicCursor implements AnfResult.Anf.ComicCursor {
        private eng: SearchEngine;
        constructor(eng: SearchEngine, keyword: string) {
            this.eng = eng;
            this.keyword = keyword;
            this.index = -1;
        }
        keyword?: string;
        index: number;
        take: number;
        current?: AnfResult.Anf.SearchComicResult;
        moveNext(): Promise<boolean> {
            if (this.index >= this.eng.length) {
                return new Promise(x => false);
            }
            this.index++;
            this.eng[this.index].create()
        }

    }
}