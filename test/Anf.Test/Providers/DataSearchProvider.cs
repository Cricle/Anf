using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test.Providers
{
    internal class DataSearchProvider : ISearchProvider
    {
        public Dictionary<string, SearchComicResult> Datas { get; set; }

        public string EngineName { get; } = "any";

        public Task<SearchComicResult> SearchAsync(string keywork, int skip, int take)
        {
            if (Datas.TryGetValue(keywork,out var val))
            {
                return Task.FromResult(val);
            }
            return Task.FromResult<SearchComicResult>(null);
        }
    }
    internal class DataSearchProvider2 : ISearchProvider
    {
        public Dictionary<string, SearchComicResult> Datas { get; set; }

        public string EngineName { get; } = "any2";

        public Task<SearchComicResult> SearchAsync(string keywork, int skip, int take)
        {
            if (Datas.TryGetValue(keywork, out var val))
            {
                return Task.FromResult(val);
            }
            return Task.FromResult<SearchComicResult>(null);
        }
    }
}
