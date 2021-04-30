using Anf.Engine;
using Anf.Networks;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.KnowEngines.ProposalProviders
{
    public class Dm5ProposalProvider : IProposalProvider
    {
        internal static readonly string Home = "http://www.dm5.com/";

        public string EngineName { get; } = "Dm5";

        private readonly INetworkAdapter networkAdapter;

        public Dm5ProposalProvider(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        protected virtual Task<Stream> GetStreamAsync(string address)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = address,
                Host = new Uri(address).Host,
                Referrer = Home
            });
        }
        public async Task<ComicSnapshot[]> GetProposalAsync(int take)
        {
            var str = string.Empty;
            using (var stream = new StreamReader(await GetStreamAsync(Home)))
            {
                str = stream.ReadToEnd();
            }
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(str);
            var sns = new List<ComicSnapshot>();
            void Fetch(int index)
            {
                if (sns.Count >= take)
                {
                    return;
                }
                var roots = htmlDoc.DocumentNode.SelectNodes($"//div[@id='index-update-{index}']/div/ul/li/div/div[@class='mh-tip-wrap']/div");
                foreach (var root in roots)
                {
                    if (sns.Count >= take)
                    {
                        return;
                    }
                    var a = root.SelectSingleNode("a");

                    var addr = a.Attributes["href"].Value;

                    var coverBlock = root.SelectSingleNode("a/p");
                    var cover = coverBlock.Attributes["style"].Value;
                    var coverLeft = cover.IndexOf('(');
                    var coverRight = cover.LastIndexOf(')');
                    var coverAddr = cover.Substring(coverLeft + 1, coverRight - coverLeft - 1);

                    var titleBlock = root.SelectSingleNode("div[@class='mh-item-tip-detali']/h2/a");
                    var title = titleBlock.Attributes["title"].Value;

                    var authBlock = root.SelectSingleNode("div[@class='mh-item-tip-detali']/p[@class='author']/span/a");
                    var auth = authBlock.InnerText?.Trim();

                    var descBlock = root.SelectSingleNode("div[@class='mh-item-tip-detali']/div[@class='desc']");
                    var desc = descBlock.InnerText.Trim();

                    var sn = new ComicSnapshot
                    {
                        Author = auth,
                        Descript = desc,
                        ImageUri = coverAddr,
                        Name = title,
                        TargetUrl = Home,
                        Sources = new ComicSource[]
                        {
                            new ComicSource
                            {
                                TargetUrl=Home+addr,
                                Name=EngineName
                            }
                        }
                    };
                    sns.Add(sn);
                }
            }
            for (int i = 1; i < 7; i++)
            {
                Fetch(i);
            }
            return sns.ToArray();
        }
    }
}
