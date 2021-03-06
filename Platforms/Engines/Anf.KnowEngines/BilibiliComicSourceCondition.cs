using Anf.Engine.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.KnowEngines
{
    [ComicSourceCondition]
    public class BilibiliComicSourceCondition : ComicSourceConditionBase<BilibiliComicOperator>
    {
        public override string EngineName => "Bilibili";

        public override Uri Address { get; } = new Uri("http://manga.bilibili.com");

        public override Uri FaviconAddress { get; } = new Uri("https://www.bilibili.com/favicon.ico");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
