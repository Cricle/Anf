using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Anf.Engine;
using Anf.Platform.Engines;
using Anf.Platform.Services;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform
{
    public class StoreComicVisiting<T> : ComicVisiting<T>
    {
        public static readonly TimeSpan DefaultDelayTime = TimeSpan.FromSeconds(1.5);
        
        public StoreComicVisiting(IServiceProvider host, IResourceFactoryCreator<T> resourceFactoryCreator)
            : base(host, resourceFactoryCreator)
        {
            storeService = AppEngine.GetRequiredService<IStoreService>();
            recyclableMemoryStreamManager = AppEngine.GetRequiredService<RecyclableMemoryStreamManager>();
            DelayTime = DefaultDelayTime;
        }

        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        private readonly IStoreService storeService;

        public virtual bool UseStore { get; set; }

        public virtual bool EnableCDNCache { get; set; }

        public virtual bool EnableRemote { get; set; }

        public virtual TimeSpan DelayTime { get; set; }

        protected virtual CloudflareCDNCacheFetcher GetCDNFetcher()
        {
            return AppEngine.GetService<CloudflareCDNCacheFetcher>();
        }

        protected override async Task<ComicPage[]> GetPagesAsync(ComicChapter chapter)
        {
            if (EnableCDNCache)
            {
                var val = await FetchCDNAsAsync<ComicPage[]>(chapter.TargetUrl);
                if (val != null)
                {
                    return val;
                }
            }
            ComicPage[] res = null;
            if (UseStore)
            {
                res = await GetAsAsync<ComicPage[]>(chapter.TargetUrl);
            }
            if (res is null)
            {
                var remoteFetch = AppEngine.GetService<RemoteEngine>();
                if (remoteFetch != null && EnableRemote)
                {
                    res = await RemoteFetchPagesAsync(remoteFetch, chapter.TargetUrl);
                    if(res is null)
                    {
                        res = await base.GetPagesAsync(chapter);
                    }
                }
                else
                {
                    res = await base.GetPagesAsync(chapter);
                }
            }
            if (res != null && EnableCDNCache)
            {
                await StoreCDNAsync(chapter.TargetUrl, res);
            }
            return res;
        }
        private async Task StoreCDNAsync<TValue>(string address, TValue value)
        {
            var remoteCacheFetcher = GetCDNFetcher();
            if (remoteCacheFetcher != null)
            {
                try
                {
                    var data = JsonHelper.Serialize(value);
                    using (var mem = recyclableMemoryStreamManager.GetStream())
                    {
                        var buffer = Encoding.UTF8.GetBytes(data);
                        mem.Write(buffer, 0, buffer.Length);
                        mem.Seek(0, SeekOrigin.Begin);
                        await remoteCacheFetcher.SaveAsync(address, mem);
                    }
                }
                catch (Exception) { }
            }
        }
        private Task<ComicEntity> RemoteFetchChapterAsync(RemoteEngine remote,string url)
        {
            return RemoteFetchAsync(remote, r => r.GetChaptersAsync(url));
        }
        private async Task<TEntity> RemoteFetchAsync<TEntity>(RemoteEngine remote,Func<RemoteEngine,Task<TEntity>> fetch)
        {
            var delayTask = Task.Delay(DelayTime);
            var remoteFetchTask = fetch(remote);
            var tasks = new Task[]
            {
                delayTask,
                remoteFetchTask
            };
            var t = await Task.WhenAny(tasks);
            if (t != delayTask && remoteFetchTask.Result != null)
            {
                return remoteFetchTask.Result;
            }
            return default;
        }
        private Task<ComicPage[]> RemoteFetchPagesAsync(RemoteEngine remote,string url)
        {
            return RemoteFetchAsync(remote, r =>r.GetPagesAsync(url, Entity.ComicUrl));
        }
        private async Task<Stream> FetchCDNAsync(string address)
        {
            var remoteCacheFetcher = GetCDNFetcher();
            if (EnableCDNCache && remoteCacheFetcher != null)
            {
                try
                {
                    return await remoteCacheFetcher.GetStreamAsync(address);
                }
                catch (Exception) { }
            }
            return null;
        }
        private async Task<TValue> FetchCDNAsAsync<TValue>(string address)
        {
            var stream = await FetchCDNAsync(address);
            if (stream is null || !stream.CanRead)
            {
                return default;
            }
            try
            {
                using (stream)
                using (var sr = new StreamReader(stream))
                {
                    var str = sr.ReadToEnd();
                    if (str.StartsWith("{"))
                    {
                        using (var obj = JsonVisitor.FromString(str))
                        {
                            if (string.Equals(obj["success"]?.ToString(), "false", StringComparison.OrdinalIgnoreCase))
                            {
                                return default;
                            }
                        }
                    }
                    return JsonHelper.Deserialize<TValue>(str);
                }
            }
            catch (Exception)
            {
                return default;
            }
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
                        return JsonHelper.Deserialize<TValue>(str);
                    }
                }
                catch (Exception) { }
            }
            return default;
        }
        protected override async Task<ComicEntity> MakeEntityAsync(string address)
        {
            if (EnableCDNCache)
            {
                var val = await FetchCDNAsAsync<ComicEntity>(address);
                if (val != null)
                {
                    return val;
                }
            }
            ComicEntity res = null;
            if (UseStore)
            {
                res = await GetAsAsync<ComicEntity>(address);
            }
            if (res is null)
            {
                var remoteFetch = AppEngine.GetService<RemoteEngine>();
                if (remoteFetch != null && EnableRemote)
                {
                    res = await RemoteFetchChapterAsync(remoteFetch, address);
                    if (res is null)
                    {
                        res = await base.MakeEntityAsync(address);
                    }
                }
                else
                {
                    res = await base.MakeEntityAsync(address);
                }
            }
            if (res != null && EnableCDNCache)
            {
                await StoreCDNAsync(address, res);
            }
            return res;
        }
    }
}
