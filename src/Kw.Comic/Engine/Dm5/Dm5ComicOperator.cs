using HtmlAgilityPack;
using JavaScriptEngineSwitcher.Core;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Dm5
{
    [EnableService(ServiceLifetime = ServiceLifetime.Scoped)]
    public class Dm5ComicOperator : IComicSourceProvider
    {
        private static readonly Regex cidRegex = new Regex(@"var DM5_CID=(.*)?;", RegexOptions.Compiled);
        private static readonly Regex dtRegex = new Regex(@"var DM5_VIEWSIGN_DT=""(.*)?"";", RegexOptions.Compiled );
        private static readonly Regex midRegex = new Regex(@"var DM5_MID=(.*)?;", RegexOptions.Compiled );
        private static readonly Regex viewSignRegex = new Regex(@"var DM5_VIEWSIGN=(.*)?;", RegexOptions.Compiled );
        private static readonly Regex imageCountRegex = new Regex(@"var DM5_IMAGE_COUNT=(.*)?;", RegexOptions.Compiled );


        private readonly HttpClient httpClient;
        private readonly IJsEngine v8;

        public Dm5ComicOperator(IHttpClientFactory clientFactory, IJsEngine v8)
        {
            this.httpClient = GetHttpClient(clientFactory);
            this.v8 = v8;
        }
        protected virtual HttpClient GetHttpClient(IHttpClientFactory clientFactory)
        {
            return clientFactory.CreateClient(ComicConst.EngineDM5);
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

            var block = html.DocumentNode.SelectNodes("//ul[@id='detail-list-select-1']/li/a");
            var titleBlock = html.DocumentNode.SelectSingleNode("//div[@class='banner_detail_form']/div[@class='info']/p[@class='title']")?.ChildNodes[0].InnerText;
            if (titleBlock==null)
            {
                titleBlock = targetUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                    .Last();
            }
            var descBlock = html.DocumentNode.SelectSingleNode("//div[@class='banner_detail_form']/div[@class='info']/p[@class='content']");
            var imgBlock = html.DocumentNode.SelectSingleNode("//div[@class='banner_detail_form']/div[@class='cover']/img");
            var caps = new List<ComicChapter>();
            foreach (var item in block)
            {
                var url = item.Attributes["href"];
                var text = item.ChildNodes[0].InnerText;
                var cap = new ComicChapter
                {
                    TargetUrl = GetBaseAddress()+ url?.Value,
                    Title = text
                };
                caps.Add(cap);
            }
            caps.Reverse();
            return new ComicEntity
            {
                Chapters = caps.ToArray(),
                Descript = descBlock?.InnerText,
                 ComicUrl=targetUrl,
                ImageUrl = imgBlock?.Attributes["src"]?.Value,
                Name = titleBlock?.Trim()
            };
        }
        protected virtual string GetBaseAddress()
        {
            return "http://www.dm5.com";
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            var str = string.Empty;
            using (var rep = await httpClient.GetAsync(targetUrl))
            {
                str = await rep.Content.ReadAsStringAsync();
            }
            var cidRgx = cidRegex.Match(str).Groups[0].Value;
            var dtRgx = dtRegex.Match(str).Groups[0].Value;
            var midRgx = midRegex.Match(str).Groups[0].Value;
            var viewSignRgx = viewSignRegex.Match(str).Groups[0].Value;
            var imgCountRgx = imageCountRegex.Match(str).Groups[0].Value;

            var cid = cidRgx.Substring(0,cidRgx.IndexOf(';')).Split('=').Last();
            var dt = dtRgx.Substring(0, dtRgx.IndexOf(';')).Split('=').Last().Trim('\"');
            var mid = midRgx.Substring(0, midRgx.IndexOf(';')).Split('=').Last();
            var viewSign = viewSignRgx.Substring(0, viewSignRgx.IndexOf(';')).Split('=').Last().Trim('\"');
            var imgCount = imgCountRgx.Substring(0, imgCountRgx.IndexOf(';')).Split('=').Last().Trim('\"');
            var val = int.Parse(imgCount);

            var refAddr = targetUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Last();
            var part = $"{refAddr}/chapterfun.ashx?cid={cid}&page={{0}}&key=&language=1&gtk=6&_cid={cid}&_mid={mid}&_dt={dt}&_sign={viewSign}";

            var pages = new List<ComicPage>();
            async Task<ComicPage[]> RunBlockAsync(int index)
            {
                var pgs = new List<ComicPage>();
                var partBlock = string.Format(part, index);
                using (var partRep = await httpClient.GetAsync(partBlock))
                {
                    var partEncod = await partRep.Content.ReadAsStringAsync();
                    var ret = v8.Evaluate<string>(partEncod);
                    var arr = ret.Split(',');
                    for (int j = 0; j < arr.Length; j++)
                    {
                        var addr = arr[j];
                        pgs.Add(new ComicPage
                        {
                            Name = (index + 1).ToString(),
                            TargetUrl = addr
                        });
                    }
                }
                return pgs.ToArray();
            }
            var maxBlocks = new List<Func<Task<ComicPage[]>>>();
            for (int i = 0; i < val; i++)
            {
                var j = i;
                maxBlocks.Add(() => RunBlockAsync(j));
            }
            var datas= await TaskQuene.RunAsync(maxBlocks.ToArray(), 5);
            var containPages = new HashSet<string>();
            foreach (var item in datas)
            {
                var res = item;
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
