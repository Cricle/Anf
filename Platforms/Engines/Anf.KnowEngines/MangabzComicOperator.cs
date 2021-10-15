using Anf.Engine.Annotations;
using Anf.Networks;
using HtmlAgilityPack;
using JavaScriptEngineSwitcher.Core;
using Jint.Native.Array;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Anf.KnowEngines
{
    [ComicSourceProvider]
    public class MangabzComicOperator : IComicSourceProvider
    {
        private static readonly Regex cidRegex = new Regex(@"var MANGABZ_CID=(.*)?;", RegexOptions.Compiled);
        private static readonly Regex dtRegex = new Regex(@"var MANGABZ_VIEWSIGN_DT=""(.*)?"";", RegexOptions.Compiled);
        private static readonly Regex midRegex = new Regex(@"var MANGABZ_MID=(.*)?;", RegexOptions.Compiled);
        private static readonly Regex viewSignRegex = new Regex(@"var MANGABZ_VIEWSIGN=(.*)?;", RegexOptions.Compiled);
        private static readonly Regex imageCountRegex = new Regex(@"var MANGABZ_IMAGE_COUNT=(.*)?;", RegexOptions.Compiled);

        private readonly INetworkAdapter networkAdapter;
        private readonly IJsEngine jsEngine;
        private static readonly Dictionary<string, string> headers = new Dictionary<string, string>
        {
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1",
        };

        public MangabzComicOperator(INetworkAdapter networkAdapter, IJsEngine jsEngine)
        {
            this.networkAdapter = networkAdapter;
            this.jsEngine = jsEngine;
        }

        protected virtual Task<Stream> GetStreamAsync(string address)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = address,
                Host = new Uri(address).Host,
                Referrer = GetBaseAddress(),
                Headers = headers
            });
        }
        protected virtual string GetBaseAddress()
        {
            return "http://www.mangabz.com/";
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
            var img = html.DocumentNode.SelectSingleNode("//div[@class='detail-info']/img[@class='detail-info-cover']");
            var title = html.DocumentNode.SelectSingleNode("//div[@class='detail-info']/p[@class='detail-info-title']");
            var desc = html.DocumentNode.SelectSingleNode("//div[@class='detail-info']/div[@class='detail-info-content']");
            
            var chpsBox = html.DocumentNode.SelectNodes("//div[@id='chapterlistload']/a");
            var chps = new List<ComicChapter>();
            foreach (var item in chpsBox.Reverse())
            {
                var name = item.InnerText.Replace("  ", string.Empty);
                var addr = item.Attributes["href"].Value;
                var chp = new ComicChapter
                {
                    Title = name,
                    TargetUrl = "http://www.mangabz.com" + addr
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
            var cidRgx = cidRegex.Match(str).Groups[0].Value;
            var dtRgx = dtRegex.Match(str).Groups[0].Value;
            var midRgx = midRegex.Match(str).Groups[0].Value;
            var viewSignRgx = viewSignRegex.Match(str).Groups[0].Value;
            var imgCountRgx = imageCountRegex.Match(str).Groups[0].Value;

            var cid = cidRgx.Substring(0, cidRgx.IndexOf(';')).Split('=').Last();
            var mid = midRgx.Substring(0, midRgx.IndexOf(';')).Split('=').Last();
            var viewSign = viewSignRgx.Substring(0, viewSignRgx.IndexOf(';')).Split('=').Last().Trim('\"');
            var imgCount = imgCountRgx.Substring(0, imgCountRgx.IndexOf(';')).Split('=').Last().Trim('\"');
            var val = int.Parse(imgCount);

            var refAddr = new Uri(targetUrl).Segments.Last();

            var part = $"{GetBaseAddress()}/{refAddr}/chapterimage.ashx?cid={cid}&page={{0}}&key=&_cid={cid}&_mid={mid}&_dt={DateTime.Now}&_sign={viewSign}";

            async Task<ComicPage[]> RunBlockAsync(int index)
            {
                var pgs = new List<ComicPage>();
                var partBlock = string.Format(part, index + 1);

                string partEncod = null;
                using (var sr = new StreamReader(await GetStreamAsync(partBlock)))
                {
                    partEncod = sr.ReadToEnd();
                }
                if (!string.IsNullOrEmpty(partEncod))
                {
                    var ret = (ArrayInstance)jsEngine.Evaluate(partEncod);
                    var length = ret.GetLength();
                    for (int i = 0; i < length; i++)
                    {
                        pgs.Add(new ComicPage
                        {
                            Name = (index + 1).ToString(),
                            TargetUrl = ret.GetProperty(i.ToString()).Value.ToString()
                        });
                    }
                }
                return pgs.ToArray();
            }
            var datas = new List<ComicPage[]>();
            for (int i = 0; i < val; i++)
            {
                var j = i;
                datas.Add(await RunBlockAsync(j));
            }
            var containPages = new HashSet<string>();
            var pages = new List<ComicPage>(datas.Count);
            for (int i = 0; i < datas.Count; i++)
            {
                var res = datas[i];
                for (int q = 0; q < res.Length; q++)
                {
                    var r = res[q];
                    if (containPages.Add(r.TargetUrl))
                    {
                        pages.Add(r);
                    }
                }
            }
            return pages.ToArray();
        }
    }
}
