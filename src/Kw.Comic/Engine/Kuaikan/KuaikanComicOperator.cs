﻿using HtmlAgilityPack;
using JavaScriptEngineSwitcher.Core;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Kuaikan
{
    [EnableService(ServiceLifetime = ServiceLifetime.Scoped)]
    public class KuaikanComicOperator : IComicSourceProvider
    {
        private static readonly Regex regex = new Regex(@"<script>window.__NUXT__=(.*)?;</script>", RegexOptions.Compiled);

        private readonly HttpClient httpClient;
        private readonly IJsEngine jsEngine;

        public KuaikanComicOperator(IHttpClientFactory factory,IJsEngine jsEngine)
        {
            this.httpClient = factory.CreateClient(ComicConst.EngineKuaiKan);
            this.jsEngine = jsEngine;
        }

        public async Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            var str = string.Empty;
            using (var rep = await httpClient.GetAsync(targetUrl))
            {
                str = await rep.Content.ReadAsStringAsync();
            }
            var html = new HtmlDocument();
            html.LoadHtml(str);
            var title = html.DocumentNode.SelectSingleNode("//div[@class='TopicList']/div/div[@class='right fl']/h3")?.InnerText;
            if (title==null)
            {
                title = targetUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            }
            var desc = html.DocumentNode.SelectSingleNode("//div[@class='comicIntro']/div[@class='details']/div/p");
            var nodes = html.DocumentNode.SelectNodes("//div[@class='TopicItem cls']");
            var img = html.DocumentNode.SelectSingleNode("//div[@class='TopicItem cls']/div[@class='left fl']/img[@class='imgCover']");
            var caps = new List<ComicChapter>();
            foreach (var item in nodes)
            {
                var a = item.SelectSingleNode("div[@class='cover fl']/a");
                var href = a.Attributes["href"];
                var imgs = a.SelectNodes("img")[1];
                var dt = imgs.Attributes["alt"];
                var chap = new ComicChapter
                {
                    TargetUrl = "https://www.kuaikanmanhua.com/"+href.Value,
                    Title = dt.Value
                };
                caps.Add(chap);
            }
            caps.Reverse();
            return new ComicEntity
            {
                Chapters = caps.ToArray(),
                Name = title,
                Descript = desc?.InnerText,
                 ComicUrl=targetUrl,
                ImageUrl = img?.Attributes["src"]?.Value
            };
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            var str = string.Empty;
            using (var rep=await httpClient.GetAsync(targetUrl))
            {
                str = await rep.Content.ReadAsStringAsync();
            }
            var jsCodesRgx = regex.Match(str);
            var jsCode = jsCodesRgx.Groups[1].Value;
            var val = jsEngine.Evaluate<string>("var a=JSON.stringify(" + jsCode+");a");
            var jobj = JObject.Parse(val);
            var info = jobj["data"][0]["res"]["data"]["comic_info"];
            var comics = (JArray)info["comic_images"];
            var pages = new List<ComicPage>();
            var title = info["title"].ToString();
            foreach (var item in comics)
            {
                var uri = item["url"].ToString();
                var page = new ComicPage
                {
                    Name = title,
                    TargetUrl = uri
                };
                pages.Add(page);
            }
            return pages.ToArray();
        }
    }
}
