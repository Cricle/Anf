﻿using HtmlAgilityPack;
using JavaScriptEngineSwitcher.Core;
using Jint.Native.Array;
using Anf;
using Anf.Networks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Anf.Engine.Annotations;

namespace Anf.KnowEngines
{
    [ComicSourceProvider]
    public class Dm5ComicOperator : IComicSourceProvider
    {
        private static readonly Regex cidRegex = new Regex(@"var DM5_CID=(.*)?;", RegexOptions.Compiled);
        private static readonly Regex dtRegex = new Regex(@"var DM5_VIEWSIGN_DT=""(.*)?"";", RegexOptions.Compiled);
        private static readonly Regex midRegex = new Regex(@"var DM5_MID=(.*)?;", RegexOptions.Compiled);
        private static readonly Regex viewSignRegex = new Regex(@"var DM5_VIEWSIGN=(.*)?;", RegexOptions.Compiled);
        private static readonly Regex imageCountRegex = new Regex(@"var DM5_IMAGE_COUNT=(.*)?;", RegexOptions.Compiled);
        private static readonly char[] targetUrlSplit = { '/' };

        protected readonly INetworkAdapter networkAdapter;
        protected readonly IJsEngine v8;

        public Dm5ComicOperator(IJsEngine v8,
            INetworkAdapter networkAdapter)
        {
            this.networkAdapter = networkAdapter;
            this.v8 = v8;
        }
        private static readonly Dictionary<string, string> headers = new Dictionary<string, string>
        {
            ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4437.0 Safari/537.36 Edg/91.0.831.1",
        };
        protected virtual Task<Stream> GetStreamAsync(string address)
        {
            return networkAdapter.GetStreamAsync(new RequestSettings
            {
                Address = address,
                Host = new Uri(address).Host
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

            var block = html.DocumentNode.SelectNodes("//ul[@id='detail-list-select-1']/li/a");
            var hideBlock= html.DocumentNode.SelectNodes("//ul[@id='detail-list-select-1']/ul[@class='chapteritem']/li/a");
            var titleBlock = html.DocumentNode.SelectSingleNode("//div[@class='banner_detail_form']/div[@class='info']/p[@class='title']")?.ChildNodes[0].InnerText;
            if (titleBlock == null)
            {
                titleBlock = targetUrl.Split(targetUrlSplit, StringSplitOptions.RemoveEmptyEntries)
                    .Last();
            }
            var descBlock = html.DocumentNode.SelectSingleNode("//div[@class='banner_detail_form']/div[@class='info']/p[@class='content']");
            var imgBlock = html.DocumentNode.SelectSingleNode("//div[@class='banner_detail_form']/div[@class='cover']/img");
            var caps = new List<ComicChapter>();
            IEnumerable<HtmlNode> blocks = block;
            if (hideBlock!=null)
            {
                blocks = blocks.Concat(hideBlock);
            }
            foreach (var item in blocks)
            {
                var url = item.Attributes["href"];
                var text = item.ChildNodes[0].InnerText;
                var cap = new ComicChapter
                {
                    TargetUrl = GetBaseAddress() + url?.Value,
                    Title = text?.Trim()
                };
                caps.Add(cap);
            }
            caps.Reverse();
            return new ComicEntity
            {
                Chapters = caps.ToArray(),
                Descript = descBlock?.InnerText,
                ComicUrl = targetUrl,
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
            var dt = dtRgx.Substring(0, dtRgx.IndexOf(';')).Split('=').Last().Trim('\"');
            var mid = midRgx.Substring(0, midRgx.IndexOf(';')).Split('=').Last();
            var viewSign = viewSignRgx.Substring(0, viewSignRgx.IndexOf(';')).Split('=').Last().Trim('\"');
            var imgCount = imgCountRgx.Substring(0, imgCountRgx.IndexOf(';')).Split('=').Last().Trim('\"');
            var val = int.Parse(imgCount);

            var refAddr = targetUrl.Split(targetUrlSplit, StringSplitOptions.RemoveEmptyEntries)
                .Last();
            var part = $"http://www.dm5.com/{refAddr}/chapterfun.ashx?cid={cid}&page={{0}}&key=&language=1&gtk=6&_cid={cid}&_mid={mid}&_dt={dt}&_sign={viewSign}";

            async Task<ComicPage[]> RunBlockAsync(int index)
            {
                var pgs = new List<ComicPage>();
                var partBlock = string.Format(part, index+1);

                string partEncod = null;
                using (var sr = new StreamReader(await GetStreamAsync(partBlock)))
                {
                    partEncod = sr.ReadToEnd();
                }
                if (!string.IsNullOrEmpty(partEncod))
                {
                    var ret = (ArrayInstance)v8.Evaluate(partEncod);
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
            var pages =new List<ComicPage>(datas.Count);
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

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return GetStreamAsync(targetUrl);
        }
    }
}
