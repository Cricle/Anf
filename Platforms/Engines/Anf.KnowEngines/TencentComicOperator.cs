using HtmlAgilityPack;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;
using Jint.Native.Array;
using Jint.Native.Object;
using Anf;
using Anf.Networks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Anf.KnowEngines
{
    public class TencentComicOperator : IComicSourceProvider
    {
        private static Regex dataRegex = new Regex(@"var ?DATA ?= ?'(.+)',", RegexOptions.Compiled);
        private static string varJs = "var window={};var W=window;var _v={};";
        private readonly INetworkAdapter networkAdapter;
        private readonly IJsEngine jsEngine;
        private static readonly Dictionary<string, string> headers = new Dictionary<string, string>
        {
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1",
        }; 
        private static readonly Dictionary<string, string> imgHeaders = new Dictionary<string, string>
        {
            ["authority"]= "manhua.acimg.cn",
            ["accept"]= "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1",
        };

        public TencentComicOperator(INetworkAdapter networkAdapter,
            IJsEngine jsEngine)
        {
            this.jsEngine = jsEngine;
            this.networkAdapter = networkAdapter;
        }
        private RequestSettings CreateSetting(string address)
        {
            return new RequestSettings
            {
                Address = address,
                Host = UrlHelper.FastGetHost(address),
                Referrer = address,
                Headers = headers
            };
        }
        private Task<Stream> XGetImageStreamAsync(string address)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = address,
                Headers = imgHeaders
            });
        }
        private Task<Stream> GetStreamAsync(string address)
        {
            return networkAdapter.GetStreamAsync(CreateSetting(address));
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

            var imgBox = html.DocumentNode.SelectSingleNode("//div[@class='works-intro clearfix']/div[@class='works-cover ui-left']/a/img");
            var targetList = html.DocumentNode.SelectNodes("//div[@id='chapter']/div[@class='works-chapter-list-wr ui-left']/ol/li/p/span/a");
            var baseQuery = "//div[@class='works-intro-text']/";
            var nameBox = html.DocumentNode.SelectSingleNode(baseQuery + "div[@class='works-intro-head clearfix']/h2[@class='works-intro-title ui-left']");
            var descBox = html.DocumentNode.SelectSingleNode("//p[@class='works-intro-short ui-text-gray9']");
            var imgLink = imgBox?.Attributes["src"]?.Value;
            var chapters = new List<ComicChapter>();
            foreach (var item in targetList)
            {
                var name = item.InnerText?.Trim();
                var link = "https://ac.qq.com" + item.Attributes["href"].Value;
                var ch = new ComicChapter { TargetUrl = link, Title = name };
                chapters.Add(ch);
            }
            return new ComicEntity
            {
                Name = nameBox?.InnerText,
                Descript = descBox?.InnerText?.Trim(),
                ComicUrl = targetUrl,
                ImageUrl = imgLink,
                Chapters = chapters.ToArray()
            };
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return XGetImageStreamAsync(targetUrl);
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            var str = string.Empty;
            using (var sr = new StreamReader(await GetStreamAsync(targetUrl)))
            {
                str = sr.ReadToEnd();
            }
            var html = new HtmlDocument();
            html.LoadHtml(str);
            var js = string.Empty;
            using (var sr=new StreamReader(await GetStreamAsync("https://ac.gtimg.com/media/js/ac.page.chapter.view_v2.6.0.js")))
            {
                js = sr.ReadToEnd();
            }
            var evalStart = js.IndexOf("eval(function(p,a,c,k,e,r)");
            var evalEnd = js.IndexOf("}();", evalStart);
            var eval = js.Substring(evalStart, evalEnd- evalStart);
            var dataBlock = dataRegex.Match(str);
            var noticLeft = str.IndexOf("window[\"n", str.IndexOf("window[\"n")+4);
            var noticRight = str.IndexOf(";", noticLeft);
            var notic = str.Substring(noticLeft, noticRight - noticLeft);
            var data = dataBlock.Groups[1].Value;
            var dataJs = "var DATA='"+data+"';window.DATA=DATA;";
            var sb = new StringBuilder();
            sb.Append(varJs);
            sb.Append(dataJs);
            sb.Append(notic+";");
            sb.Append(eval);
            sb.Append(";JSON.stringify(_v);");
            var val = jsEngine.Evaluate(sb.ToString()).ToString();
            using (var doc = JsonVisitor.FromString(val))
            {
                var pages = new List<ComicPage>();
                var pics = doc["picture"].ToArray();
                foreach (var item in pics)
                {
                    var pid = item["pid"];
                    var url = item["url"];
                    var page = new ComicPage
                    {
                        Name=pid.ToString(),
                        TargetUrl=url.ToString()
                    };
                    pages.Add(page);
                }
                return pages.ToArray();
            }
        }
    }
}
