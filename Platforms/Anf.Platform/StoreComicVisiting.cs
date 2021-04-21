using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform
{
    public class StoreComicVisiting<T> : ComicVisiting<T>
    {
        public StoreComicVisiting(IServiceProvider host, IResourceFactoryCreator<T> resourceFactoryCreator) 
            : base(host, resourceFactoryCreator)
        {
            storeService = AppEngine.GetRequiredService<IStoreService>();
        }

        private readonly IStoreService storeService;

        public bool UseStore { get; set; }

        protected override async Task<ComicPage[]> GetPagesAsync(ComicChapter chapter)
        {
            if (UseStore)
            {
                var val = await GetAsAsync<ComicPage[]>(chapter.TargetUrl);
                if (val != null)
                {
                    return val;
                }
            }
            return await base.GetPagesAsync(chapter);
        }
        private async Task<TValue> GetAsAsync<TValue>(string address)
        {
            var stream = await storeService.GetStreamAsync(address);
            if (stream != null)
            {
                try
                {
                    using (var sr = new StreamReader(stream))
                    {
                        var str = sr.ReadToEnd();
                        return JsonConvert.DeserializeObject<TValue>(str);
                    }
                }
                catch (Exception) { }
            }
            return default;
        }
        protected override async Task<ComicEntity> MakeEntityAsync(string address)
        {
            if (UseStore)
            {
                var val = await GetAsAsync<ComicEntity>(address);
                if (val != null)
                {
                    return val;
                }
            }
            return await base.MakeEntityAsync(address);
        }
    }
}
