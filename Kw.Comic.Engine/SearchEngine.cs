using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kw.Comic.Engine
{
    public class SearchEngine : List<Type>
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public SearchEngine(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<SearchComicResult> SearchAsync(string keyword, int skip, int take)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var result = new SearchComicResult { Total = 0 };
                var datas = new List<ComicSnapshot>();
                foreach (var item in this)
                {
                    var eng = (ISearchProvider)scope.ServiceProvider.GetRequiredService(item);
                    var dt = await eng.SearchAsync(keyword, skip, take);
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
