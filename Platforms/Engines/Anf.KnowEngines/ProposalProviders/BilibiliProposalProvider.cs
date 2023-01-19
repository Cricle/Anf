using Anf.Engine.Annotations;
using Anf.Networks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValueBuffer;

namespace Anf.KnowEngines.ProposalProviders
{
    [ProposalProvider]
    public class BilibiliProposalProvider : IProposalProvider
    {
        private static readonly string url = "https://manga.bilibili.com/twirp/comic.v1.Comic/HomeRecommend?device=pc&platform=web";
        public string EngineName => "Bilibili";

        private readonly INetworkAdapter networkAdapter;

        public BilibiliProposalProvider(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        public async Task<ComicSnapshot[]> GetProposalAsync(int take)
        {
            var str = string.Empty;

            using(var mem= new ValueBufferMemoryStream())
            {
                var buffer = Encoding.UTF8.GetBytes("{\"page_num\":4,\"seed\":\"0\"}");
                mem.Write(buffer,0,buffer.Length);
                mem.Seek(0, SeekOrigin.Begin);
                var datas = await networkAdapter.GetStreamAsync(new RequestSettings
                {
                    Address = url,
                    Referrer = "https://manga.bilibili.com/",
                    Method = "POST",
                    Data = mem,
                    Headers = new Dictionary<string, string>(1)
                    {
                        ["Content-Type"] = "application/json"
                    }
                });
                using (var sr = new StreamReader(datas))
                {
                    str = sr.ReadToEnd();
                }
            }
            using (var doc = JsonVisitor.FromString(str))
            {
                var dataTk = doc["data"]["list"].ToEnumerable().ToArray();
                var sns = new List<ComicSnapshot>(dataTk.Length);
                for (int i = 0; i < dataTk.Length; i++)
                {
                    if (take < i)
                    {
                        break;
                    }
                    var tk = dataTk[i];
                    var authTk = tk["authors"]?.ToEnumerable();
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
