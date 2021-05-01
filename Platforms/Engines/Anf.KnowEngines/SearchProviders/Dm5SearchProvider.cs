using Anf.Networks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.KnowEngines.SearchProviders
{
    public class Dm5SearchProvider : ISearchProvider
    {
        private static readonly string url = "http://www.dm5.com/search?title={0}&language=1";

        private readonly INetworkAdapter networkAdapter;

        public Dm5SearchProvider(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        public string EngineName => "Dm5";

        public async Task<SearchComicResult> SearchAsync(string keywork, int skip, int take)
        {
            var targetUrl = string.Format(url, keywork);
            var req = new RequestSettings
            {
                Address = targetUrl,
                Host = "www.dm5.com",
                Referrer = "http://www.dm5.com/"
            };
            var str =await networkAdapter.GetStringAsync(req);
            var html = new HtmlDocument();
            html.LoadHtml(str);
            var box = html.DocumentNode.SelectNodes("//div[@class='box-body']/ul/li/div[@class='mh-item']");
            var sns = new List<ComicSnapshot>();
            var res = new SearchComicResult
            {
                Total = box.Count,
                Support = true
            };
            foreach (var item in box.Skip(skip).Take(take))
            {
                var converBox = item.SelectSingleNode("p").Attributes["style"].Value;
                var converBoxLeft = converBox.IndexOf('(');
                var converBoxRight = converBox.LastIndexOf(')');
                var conver = converBox.Substring(converBoxLeft + 1, converBoxRight- converBoxLeft - 1);

                var detail = item.SelectSingleNode("div[@class='mh-item-detali']");
                var titleBox = detail.SelectSingleNode("h2[@class='title']/a");
                var title = titleBox.InnerText?.Trim();
                var url = "http://www.dm5.com" + titleBox.Attributes["href"].Value;

                var tipDetail = item.SelectSingleNode("div[@class='mh-tip-wrap']/div[@class='mh-item-tip']/div[@class='mh-item-tip-detali']");
                var desc = tipDetail.SelectSingleNode("div[@class='desc']");
                var auth = tipDetail.SelectSingleNode("p[@class='author']/span/a").InnerText;
                var sn = new ComicSnapshot
                {
                    Author = auth,
                    Descript = desc.InnerText,
                    ImageUri = conver,
                    Name = title,
                    TargetUrl = targetUrl,
                    Sources = new ComicSource[]
                    {
                        new ComicSource
                        {
                            TargetUrl=url,
                            Name="Dm5"
                        },
                    },
                };
                sns.Add(sn);
            }
            res.Snapshots = sns.ToArray();
            return res;
        }
    }
}
