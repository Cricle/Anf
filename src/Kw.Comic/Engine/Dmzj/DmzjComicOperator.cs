using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JavaScriptEngineSwitcher.Core;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Kw.Comic.Engine;
using Microsoft.Extensions.DependencyInjection;
using Kw.Core.Annotations;
using Newtonsoft.Json.Linq;
using System.Web;

namespace Kw.Comic.Engine.Dmzj
{
    [EnableService(ServiceLifetime = ServiceLifetime.Scoped)]
    public class DmzjComicOperator : IComicSourceProvider
    {
        private static readonly Regex regex = new Regex(@"eval\((.*?)\{.*?\}\)\)", RegexOptions.Compiled);
        
        private readonly HttpClient http;
        private readonly IJsEngine v8;


        public DmzjComicOperator(IHttpClientFactory httpClientFactory, IJsEngine v8)
        {
            http = httpClientFactory.CreateClient(ComicConst.EngineDMZJ);
            this.v8 = v8;
        }


        public static string GetTrueUrl(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                return "http://" + url;
            }
            return url;
        }

        public async Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            targetUrl = GetTrueUrl(targetUrl);
            string str = null;
            using (var rep = await http.GetAsync(targetUrl))
            {
                str = await rep.Content.ReadAsStringAsync();
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(str);
            var title = doc.DocumentNode.SelectSingleNode("//div[@class='comic_deCon']/h1/a")?.InnerText;
            if (title==null)
            {
                title = doc.DocumentNode.SelectSingleNode("//span[@class='anim_title_text']/a/h1")?.InnerText;
            }
            if (title==null)
            {
                title = targetUrl.Split('/').Last();
            }
            var desc = doc.DocumentNode.SelectSingleNode("//div[@clas='wrap_intro_l_comic']/div[@class='comic_deCon']/p[@class='comic_deCon_d']");
            var node = doc.DocumentNode.SelectNodes("//div[@class='cartoon_online_border']/ul/li/a");
            if (node == null)
            {
                node = doc.DocumentNode.SelectNodes("//ul[@class='list_con_li autoHeight']/li/a");
            }
            var img = doc.DocumentNode.SelectSingleNode("//div[@class='wrap_intro_l_comic']/div[@class='comic_i']/div[@class='comic_i_img']/a/img");
            var chartps = new List<ComicChapter>();
            for (int i = 0; i < node.Count; i++)
            {
                var item = node[i];
                var name = item.InnerText;
                var url = item.Attributes["href"]?.Value;
                if (url!=null&& !url.StartsWith("http://")
                    && !url.StartsWith("https://"))
                {
                    url = "https://manhua.dmzj.com" + url;
                }
                var chap = new ComicChapter
                {
                    TargetUrl = url,
                    Title = HttpUtility.UrlDecode(name)
                };
                chartps.Add(chap);
            }
            chartps.Reverse();
            var entity = new ComicEntity
            {
                Chapters = chartps.ToArray(),
                Descript = desc?.InnerText,
                ImageUrl = img?.Attributes["src"]?.Value,
                Name = title?.Trim()
            };
            return entity;
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            var blocks = new List<ComicPage>();
            string str = null;
            using (var rep = await http.GetAsync(targetUrl))
            {
                str = await rep.Content.ReadAsStringAsync();
            }
            var match = regex.Match(str);
            {
                var sc = match.Groups[0].Value + ";pages;";
                var strx = v8.Evaluate(sc)?.ToString();
                string[] inn = null;
                if (strx.StartsWith("{"))
                {
                    var doc = JObject.Parse(strx);
                    inn = doc["page_url"].ToString().Split('\n');
                }
                else
                {
                    var doc = JArray.Parse(strx);
                    inn = doc.Select(x => x.ToString()).ToArray();
                }
                foreach (var item in inn)
                {
                    var val = item.ToString().Trim();
                    var name = HttpUtility.UrlDecode(val.Split('/').Last());
                    blocks.Add(new ComicPage
                    {
                        Name = name,
                        TargetUrl = "https://images.dmzj.com/" + val
                    });
                }
            }
            return blocks.ToArray();
        }
    }
}
