using Anf.Networks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Anf.KnowEngines
{
    public class BikabikaComicOperator : IComicSourceProvider
    {
        private static readonly Regex urlsRegex = new Regex(@"var qTcms_S_m_murl_e=(.*)?;", RegexOptions.Compiled);

        private readonly INetworkAdapter networkAdapter;

        private static readonly Dictionary<string, string> headers = new Dictionary<string, string>
        {
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1",
        };

        public BikabikaComicOperator(INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
        }

        protected virtual Task<Stream> GetStreamAsync(string address)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = address,
                Host = new Uri(address).Host,
                Referrer= "http://www.bikabika.com/",
                Headers = headers
            });
        }
        public async Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            var str = string.Empty;
            using (var sr = new StreamReader(await GetStreamAsync(targetUrl)))
            {
                str = sr.ReadToEnd();
            }
            var html = new HtmlDocument();
            html.LoadHtml(str);
            var img = html.DocumentNode.SelectSingleNode("//div[@class='banner_detail_form']/div[@class='cover']/img");
            var title = html.DocumentNode.SelectSingleNode("//div[@class='banner_detail_form']/div[@class='info']/p[@class='title']");
            var desc = html.DocumentNode.SelectSingleNode("//p[@id='a_closes']");

            var chpsBox = html.DocumentNode.SelectNodes("//div[@id='chapterlistload']/ul/li/a");
            var chps = new List<ComicChapter>();
            foreach (var item in chpsBox)
            {
                var name = item.InnerText.Trim();
                var addr = item.Attributes["href"].Value;
                var chp = new ComicChapter
                {
                    Title = name,
                    TargetUrl = "http://www.bikabika.com" + addr
                };
                chps.Add(chp);
            }
            var entity = new ComicEntity
            {
                ComicUrl = targetUrl,
                Descript = desc?.InnerText,
                ImageUrl = img?.Attributes["src"].Value,
                Name = title?.InnerText,
                Chapters = chps.ToArray()
            };
            return entity;
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return GetStreamAsync(targetUrl);
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            var str = string.Empty;
            using (var sr = new StreamReader(await GetStreamAsync(targetUrl)))
            {
                str = sr.ReadToEnd();
            }
            var g = urlsRegex.Match(str);
            var urlRgx = urlsRegex.Match(str).Groups[0].Value;
            var firstEqual = urlRgx.IndexOf('\"');
            var lastEqual = urlRgx.LastIndexOf('\"');
            var cid = urlRgx.Substring(firstEqual+1,lastEqual-firstEqual-1);
            var urlStr = Base64Decode(Encoding.UTF8, cid);
            var urls = urlStr.Split(new string[] { "$qingtiandy$" }, StringSplitOptions.RemoveEmptyEntries);
            var index = 1;
            return urls.Select(x => new ComicPage { TargetUrl = x, Name = index++.ToString() })
                .ToArray();
        }
        private static string Base64Decode(Encoding encodeType, string result)
        {
            return encodeType.GetString(Convert.FromBase64String(result));
        }
    }
}
