using Anf.Engine;
using Anf.Networks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.KnowEngines.ProposalProviders
{
    public class BilibiliProposalDescription : IProposalDescription
    {
        public Type ProviderType { get; } = typeof(BilibiliProposalProvider);

        public string Name { get; } = "Bilibili";

        public Uri DescritionUri { get; } = new Uri("https://manga.bilibili.com/");
    }
    public class BilibiliProposalProvider : IProposalProvider
    {
        private static readonly string url = "https://manga.bilibili.com/twirp/comic.v1.Comic/HomeHot?device=pc&platform=web";
        public string EngineName => "Bilibili";

        private readonly INetworkAdapter networkAdapter;

        public BilibiliProposalProvider(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        protected virtual Task<Stream> GetStreamAsync(string address, string method = null)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = address,
                Referrer = "https://manga.bilibili.com/",
                Method = method
            });
        }
        public async Task<ComicSnapshot[]> GetProposalAsync(int take)
        {
            var datas = await GetStreamAsync(url, "post");
            var str = string.Empty;
            using (var sr = new StreamReader(datas))
            {
                str = sr.ReadToEnd();
            }
            using (var doc = JsonVisitor.FromString(str))
            {
                var dataTk = doc["data"].ToArray().ToArray();
                var sns = new List<ComicSnapshot>(dataTk.Length);
                for (int i = 0; i < dataTk.Length; i++)
                {
                    if (take < i)
                    {
                        break;
                    }
                    var tk = dataTk[i];
                    var authTk = tk["author"].ToArray();
                    var title = tk["title"]?.ToString();
                    var id = tk["comic_id"]?.ToString();
                    var conver = tk["vertical_cover"]?.ToString();
                    var auth = string.Join("-", authTk.Select(x => x.ToString()));
                    var sn = new ComicSnapshot
                    {
                        Author = auth,
                        Name = title,
                        ImageUri = conver,
                        TargetUrl = url,
                        Sources = new ComicSource[]
                        {
                        new ComicSource
                        {
                            Name=EngineName,
                            TargetUrl="https://manga.bilibili.com/detail/"+ id
                        }
                        }
                    };
                    sns.Add(sn);
                }
                return sns.ToArray();
            }
        }
    }
}
