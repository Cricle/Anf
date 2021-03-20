using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kw.Comic.Engine
{
    public static class SearchEngineExtensions
    {
        class DefaultComicCursor : IComicCursor
        {
            private readonly IServiceScope scope;
            private readonly IEnumerator<Type> engineTypes;
            private SearchComicResult current;

            public DefaultComicCursor(IServiceScope scope,
                IEnumerator<Type> engineTypes,
                string keyword,
                int index,
                int take)
            {
                this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
                this.engineTypes = engineTypes ?? throw new ArgumentNullException(nameof(engineTypes));
                Keyword = keyword;
                Index = index;
                Take = take;
            }
            public string Keyword { get; }

            public int Index { get; }

            public int Take { get; }

            public Type CurrentSearchEngine => engineTypes.Current;

            public SearchComicResult Current => current;

            public void Dispose()
            {
                scope.Dispose();
            }

            public async Task<bool> MoveNextAsync()
            {
                if (engineTypes.MoveNext())
                {
                    var provider = (ISearchProvider)scope.ServiceProvider.GetRequiredService(engineTypes.Current);
                    current = await provider.SearchAsync(Keyword, Index * Take, Take);
                }
                return false;
            }
        }
        public static Task<IComicCursor> GetSearchCursorAsync(this SearchEngine eng, string keyword, int skip = 0, int take = 50)
        {
            var scope = eng.ServiceScopeFactory.CreateScope();
            return Task.FromResult<IComicCursor>(new DefaultComicCursor(scope, eng.GetEnumerator(), keyword, skip, take));
        }
        public static async Task<SearchComicResult> SearchAsync(this SearchEngine eng, string keyword, int skip, int take)
        {
            using (var scope = eng.ServiceScopeFactory.CreateScope())
            {
                var result = new SearchComicResult { Total = 0 };
                var datas = new List<ComicSnapshot>();
                foreach (var item in eng)
                {
                    var prov = (ISearchProvider)scope.ServiceProvider.GetRequiredService(item);
                    var dt = await prov.SearchAsync(keyword, skip, take);
                    if (dt.Support && dt.Snapshots != null)
                    {
                        datas.AddRange(dt.Snapshots);
                        result.Total += dt.Total;
                    }
                }
                result.Snapshots = datas.ToArray();
                return result;
            }
        }
    }
}
