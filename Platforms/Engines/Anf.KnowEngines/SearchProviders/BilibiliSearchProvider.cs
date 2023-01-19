using Anf.Engine;
using Anf.Engine.Annotations;
using Anf.Networks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueBuffer;

namespace Anf.KnowEngines.SearchProviders
{
    [ComicSearchProvider]
    public class BilibiliSearchProvider : ISearchProvider
    {
        private static readonly string searchUrl = "https://manga.bilibili.com/twirp/comic.v1.Comic/Search?device=pc&platform=web";

        public string EngineName => "Bilibili";
        private readonly INetworkAdapter networkAdapter;
        private static readonly IReadOnlyDictionary<string, string> headers = new Dictionary<string, string>
        {
            ["Content-Type"] = "application/json",
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1",

        };

        public BilibiliSearchProvider(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        private Stream GetStream()
        {
            return new ValueBufferMemoryStream();
        }

        public async Task<SearchComicResult> SearchAsync(string keywork, int skip, int take)
        {
            var pageIndex = 1;
            if (take!=0)
            {
                pageIndex = Math.Max(1, skip / take);
            }
            var searchObj = new
            {
                key_word = keywork,
                page_num = pageIndex,
                page_size = take
            };
            var searchStr = JsonHelper.Serialize(searchObj);
            var res = new SearchComicResult
            {
                Support = true
            };
            using (var mem = GetStream())
            {
                var buffer = Encoding.UTF8.GetBytes(searchStr);
                mem.Write(buffer,0,buffer.Length);
                mem.Seek(0, SeekOrigin.Begin);
                var req = new RequestSettings
                {
                    Address = searchUrl,
                    Method = "POST",
                    Host=UrlHelper.FastGetHost(searchUrl),
                    Referrer = "https://manga.bilibili.com/",
                    Data = mem,
                    Headers = headers
                };
                var str =await networkAdapter.GetStringAsync(req);
                using (var visitor = JsonVisitor.FromString(str))
                {
                    var list = visitor["data"]["list"].ToEnumerable();
                    res.Total = list.Count();
                    var sns = new List<ComicSnapshot>();
                    foreach (var item in list)
                    {
                        var conv = item["vertical_cover"].ToString();
                        var title = item["org_title"].ToString();
                        var id = item["id"].ToString();
                        var auth = string.Join(",", item["author_name"].ToEnumerable());
                        var url = "https://manga.bilibili.com/detail/mc" + id;
                        var sn = new ComicSnapshot
                        {
                            Author = auth,
                            ImageUri = conv,
                            Name = title,
                            TargetUrl = url,
                            Sources = new ComicSource[]
                            {
                                new ComicSource
                                {
                                    TargetUrl=url,
                                    Name="Bilibili"
                                }
                            }
                        };
                        sns.Add(sn);
                    }
                    res.Snapshots = sns.ToArray();
                }
            }
            return res;
        }
    }
}
