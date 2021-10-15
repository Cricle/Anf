using Anf.Engine.Annotations;
using Anf.Networks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.KnowEngines
{
    [ComicSourceProvider]
    public class QimiaoComicOperator : IComicSourceProvider
    {
        private static readonly string pagesUrl = "https://www.qimiaomh.com/Action/Play/AjaxLoadImgUrl?did={0}&sid={1}";
        private readonly INetworkAdapter networkAdapter;

        public QimiaoComicOperator(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        protected virtual Task<Stream> GetStreamAsync(string address)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = address,
                Host = new Uri(address).Host,
                Referrer = "https://www.qimiaomh.com/",
            });
        }

        public async Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            using (var stream=await GetStreamAsync(targetUrl))
            {
                var str = string.Empty;
                using (var sr=new StreamReader(stream))
                {
                    str = sr.ReadToEnd();
                }
                var html = new HtmlDocument();
                html.LoadHtml(str);
                var img = html.DocumentNode.SelectSingleNode("//div[@class='inner']/div[@class='ctdbLeft ']/a/img");
                var desc = html.DocumentNode.SelectSingleNode("//p[@id='worksDesc']");
                var list = html.DocumentNode.SelectNodes("//div[@class='comic-content-list']/ul/li/a");
                var chps = new List<ComicChapter>();
                foreach (var item in list)
                {
                    var url = item.Attributes["href"].Value;
                    var name = item.Attributes["title"].Value;
                    var chp = new ComicChapter
                    {
                        TargetUrl = "https://www.qimiaomh.com" + url,
                        Title = name
                    };
                    chps.Add(chp);
                }
                var entity = new ComicEntity
                {
                    Name = img.Attributes["alt"].Value,
                    Chapters = chps.ToArray(),
                    ComicUrl = targetUrl,
                    Descript = desc?.InnerText,
                    ImageUrl = img.Attributes["src"].Value
                };
                return entity;
            }
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return GetStreamAsync(targetUrl);
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            var tur = new Uri(targetUrl).Segments;
            var seg = tur[tur.Length - 2].Trim('/');
            var idx = tur[tur.Length - 1].Replace(".html",string.Empty);
            var url = string.Format(pagesUrl, seg, idx);
            using (var stream =await GetStreamAsync(url))
            {
                var str = string.Empty;
                using (var sr=new StreamReader(stream))
                {
                    str = sr.ReadToEnd();
                }
                using (var jobj=JsonVisitor.FromString(str))
                {
                    var imgs = jobj["listImg"].ToArray();
                    var pages = new List<ComicPage>();
                    var i = 0;
                    foreach (var item in imgs)
                    {
                        var page = new ComicPage { Name = i++.ToString(), TargetUrl = item.ToString() };
                        pages.Add(page);
                    }
                    return pages.ToArray();
                }
            }
        }
    }
}
