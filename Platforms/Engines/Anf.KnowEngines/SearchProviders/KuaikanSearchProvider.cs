using Anf.Engine.Annotations;
using Anf.Networks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.KnowEngines.SearchProviders
{
    [ComicSearchProvider]
    public class KuaikanSearchProvider : ISearchProvider
    {
        private readonly static string url = "https://www.kuaikanmanhua.com/v1/search/topic?q={0}&f={1}&size={2}";

        public string EngineName => "Kuakan";

        private readonly INetworkAdapter networkAdapter;

        public KuaikanSearchProvider(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        public async Task<SearchComicResult> SearchAsync(string keywork, int skip, int take)
        {
            var page = 1;
            if (take!=0)
            {
                page = Math.Max(1, skip / take);
            }
            var targetUrl = string.Format(url, keywork, page, take);
            var setting = new RequestSettings
            {
                Address=targetUrl,
                 Referrer= "https://www.kuaikanmanhua.com/"
            };
            var datas = await networkAdapter.GetStringAsync(setting);
            using (var visit=JsonVisitor.FromString(datas))
            {
                var hit = visit["data"]["hit"].ToEnumerable();
                var res = new SearchComicResult
                {
                    Total = hit.Count(),
                    Support = true
                };
                var sns = new List<ComicSnapshot>();
                foreach (var item in hit)
                {
                    var conv = item["vertical_image_url"].ToString();
                    var title = item["title"].ToString();
                    var id = item["id"].ToString();
                    var desc = item["description"].ToString();
                    var user = item["user"]["nickname"].ToString();
                    var sn = new ComicSnapshot
                    {
                        Author = user,
                        Descript = desc,
                        ImageUri = conv,
                        Name = title,
                        TargetUrl = targetUrl,
                        Sources = new ComicSource[]
                        {
                            new ComicSource
                            {
                                TargetUrl="https://www.kuaikanmanhua.com/web/topic/"+id,
                                Name="Kuaikan"
                            }
                        }
                    };
                    sns.Add(sn);
                }
                res.Snapshots = sns.ToArray();
                return res;
            }
        }
    }
}
