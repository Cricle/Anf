using Anf.Networks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Anf;
using Anf.Engine.Annotations;

namespace Anf.KnowEngines.SearchProviders
{
    [ComicSearchProvider]
    public class SomanSearchProvider : ISearchProvider
    {
        public const string SeachUrl = "http://api.soman.com/soman.ashx?action=getsomancomics2&pageindex={0}&pagesize={1}&keyword={2}&time={3}";

        private readonly INetworkAdapter networkAdapter;

        public SomanSearchProvider(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        public string EngineName => "Soman";

        private static readonly DateTime utcZero = new DateTime(1970, 1, 1);

        public static TimeSpan UTCSpan(DateTime time)
        {
            return time.ToUniversalTime() - utcZero;
        }
        public async Task<SearchComicResult> SearchAsync(string keywork, int skip, int take)
        {
            var page = 1;
            if (skip != 0 && skip > take)
            {
                page = take / skip;
            }
            var targetUrl = string.Format(SeachUrl, page, take, keywork, (int)UTCSpan(DateTime.Now).TotalSeconds);
            string str = string.Empty;
            using (var rep = await networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = targetUrl,
                Referrer = "https://www.soman.com/"
            }))
            using (var sr = new StreamReader(rep))
            {
                str = sr.ReadToEnd();
            }
            var visitor = JsonVisitor.FromString(str);
            try
            {

                var total = int.Parse(visitor["Total"].ToString());
                var items = visitor["Items"].ToEnumerable();
                var snaps = new List<ComicSnapshot>();
                foreach (var item in items)
                {
                    var comic = item["Comics"].ToEnumerable();
                    if (!comic.Any())
                    {
                        continue;
                    }
                    var sn = new ComicSnapshot();
                    var sources = new List<ComicSource>();
                    foreach (var c in comic)
                    {
                        var host = c["Host"];
                        var part = c["Url"];
                        var name = c["Source"];
                        var source = new ComicSource
                        {
                            Name = name.ToString(),
                            TargetUrl = host.ToString() + part.ToString()
                        };
                        sources.Add(source);
                    }
                    var first = comic.First();
                    sn.Name = first["SomanId"].ToString();
                    sn.ImageUri = first["PicUrl"].ToString();
                    sn.Author = first["Author"].ToString();
                    sn.Descript = first["Content"].ToString();
                    sn.TargetUrl = targetUrl;
                    sn.Sources = sources.ToArray();
                    snaps.Add(sn);
                }
                return new SearchComicResult
                {
                    Snapshots = snaps.ToArray(),
                    Support = true,
                    Total = total
                };
            }
            finally
            {
                visitor.Dispose();
            }
        }
    }
}
