using Anf.Networks;
using HtmlAgilityPack;
using JavaScriptEngineSwitcher.Core;
using Jint.Native.Array;
#if !NETSTANDARD1_3
using Microsoft.IO;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Anf.KnowEngines
{
    public class BilibiliComicOperator : IComicSourceProvider
    {
        private static readonly string detailUri = "https://manga.bilibili.com/twirp/comic.v1.Comic/ComicDetail?device=pc&platform=web";
        private static readonly string imgIndexUri = "https://manga.bilibili.com/twirp/comic.v1.Comic/GetImageIndex?device=pc&platform=web";
        private static readonly string imgTokenUri = "https://manga.bilibili.com/twirp/comic.v1.Comic/ImageToken?device=pc&platform=web";

        private readonly INetworkAdapter networkAdapter;
        private static readonly Dictionary<string, string> headers = new Dictionary<string, string>
        {
            ["Content-Type"] = "application/json",
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1",
        };
#if !NETSTANDARD1_3
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        public BilibiliComicOperator(INetworkAdapter networkAdapter, RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        {
            this.networkAdapter = networkAdapter;
            this.recyclableMemoryStreamManager = recyclableMemoryStreamManager;
        }
#else
        public BilibiliComicOperator(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }
#endif

        protected virtual Task<Stream> GetStreamAsync(string address,string method, Stream stream = null)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = address,
                Host = new Uri(address).Host,
                Referrer = "https://manga.bilibili.com/",
                Headers = headers,
                Method=method,
                Data = stream
            });
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Stream GetStream()
        {
#if NETSTANDARD1_3
            return new MemoryStream();
#else
            return recyclableMemoryStreamManager.GetStream();
#endif
        }
        private void WrtieStream(Stream stream,string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);
        }
        public async Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            var mc = new Uri(targetUrl).Segments.Last();
            var part = mc.Remove(0, 2);
            using (var mem =GetStream())
            {
                var arg = $"{{\"comic_id\":{part}}}";
                WrtieStream(mem, arg);
                var stream = await GetStreamAsync(detailUri,"POST", mem);
                string str = null;
                using (var sr = new StreamReader(stream))
                {
                    str = sr.ReadToEnd();
                }
                using (var jobj = JsonVisitor.FromString(str))
                {
                    var data = jobj["data"];
                    var entity = new ComicEntity
                    {
                        Name = data["title"].ToString(),
                        Descript = data["evaluate"].ToString(),
                        ComicUrl = targetUrl,
                        ImageUrl = data["vertical_cover"].ToString()
                    };
                    var chapts = new List<ComicChapter>();
                    var ep = data["ep_list"].ToArray();
                    foreach (var item in ep)
                    {
                        var title = item["title"].ToString();
                        var id = item["id"].ToString();
                        var url = "https://manga.bilibili.com/" + mc + "/" + id;
                        var chp = new ComicChapter
                        {
                            TargetUrl = url,
                            Title = title
                        };
                        chapts.Add(chp);
                    }
                    chapts.Reverse();
                    entity.Chapters = chapts.ToArray();
                    return entity;
                }
            }

        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return GetStreamAsync(targetUrl, null);
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            const int width = 660;
            var epId = new Uri(targetUrl).Segments.Last();
            string targetObjStr = null;
            using (var mem = GetStream())
            {
                var arg = $"{{\"ep_id\":{epId}}}";
                WrtieStream(mem, arg);
                var stream = await GetStreamAsync(imgIndexUri, "POST", mem);
                string str = null;
                using (var sr = new StreamReader(stream))
                {
                    str = sr.ReadToEnd();
                }
                using (var jobj=JsonVisitor.FromString(str))
                {
                    var imgs = jobj["data"]["images"].ToArray();
                    var paths = imgs.Select(x => x["path"].ToString()+$"@{width}w.jpg").ToArray();
                    targetObjStr = "{urls:\"[\"" + string.Join(",", paths) + "\"]\"}";
                    //targetObj = JObject.FromObject(new
                    //{
                    //    urls="[\""+string.Join(",",paths)+"\"]"
                    //});
                }
            }
            using (var mem = GetStream())
            {
                WrtieStream(mem, targetObjStr);
                var stream = await GetStreamAsync(imgTokenUri, "POST", mem);
                string str = null;
                using (var sr = new StreamReader(stream))
                {
                    str = sr.ReadToEnd();
                }
                using (var jobj=JsonVisitor.FromString(str))
                {
                    var data = jobj["data"].ToArray();
                    var pages = data.Select(x => x["url"].ToString() + "?token=" + x["token"].ToString())
                        .ToArray();
                    return Enumerable.Range(0, pages.Length)
                        .Select(x => new ComicPage { Name = x.ToString(), TargetUrl = pages[x] })
                        .ToArray();
                }
            }
        }
    }
}
