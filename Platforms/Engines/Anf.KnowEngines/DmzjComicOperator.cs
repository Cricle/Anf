﻿using HtmlAgilityPack;
using JavaScriptEngineSwitcher.Core;
using Anf;
using Anf.Networks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Anf.Engine.Annotations;
using System;

namespace Anf.KnowEngines
{
    [ComicSourceProvider]
    public class DmzjComicOperator : IComicSourceProvider
    {
        private static readonly Regex regex = new Regex(@"eval\((.*?)\{.*?\}\)\)", RegexOptions.Compiled);
        
        private readonly IJsEngine v8;
        private readonly INetworkAdapter networkAdapter;

        public DmzjComicOperator(IJsEngine v8, INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
            this.v8 = v8;
        }
        private Task<Stream> CreateRequest(string url)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address=url,
                Referrer= "https://www.dmzj.com/"
            });
        }
        private Task<Stream> CreateImageRequest(string url)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = url,
                Referrer = "https://www.dmzj.com/",
                Host= "images.dmzj1.com"
            });
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
            using (var sr=new StreamReader(await CreateRequest(targetUrl)))
            {
                str = sr.ReadToEnd();
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
            for (int i = node.Count-1; i >= 0; i--)
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
                 ComicUrl=targetUrl,
                ImageUrl = img?.Attributes["src"]?.Value,
                Name = title?.Trim()
            };
            return entity;
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            var blocks = new List<ComicPage>();
            string str = null;
            using (var sr = new StreamReader(await CreateRequest(targetUrl)))
            {
                str = sr.ReadToEnd();
            }
            var match = regex.Match(str);
            var sc = match.Groups[0].Value + ";pages;";
            var strx = v8.Evaluate(sc)?.ToString();
            if (strx is null)
            {
#if NETSTANDARD1_3 || NET452
                    return new ComicPage[0];
#else
                return Array.Empty<ComicPage>();
#endif
            }
            string[] inn = null;
            var visitor = JsonVisitor.FromString(strx.Replace("\r\n", "$@$"));
            try
            {

                if (strx.StartsWith("{"))
                {
                    inn = visitor["page_url"].ToString().Split(new[] { "$@$" }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    inn = visitor.ToEnumerable().Select(x => x.ToString()).ToArray();
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
            finally
            {
                visitor.Dispose();
            }
            return blocks.ToArray();
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return CreateImageRequest(targetUrl);
        }
    }
}
