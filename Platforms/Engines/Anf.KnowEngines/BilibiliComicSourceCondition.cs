using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.KnowEngines
{
    public class BilibiliComicSourceCondition : ComicSourceConditionBase<BilibiliOperator>
    {
        public override string EnginName => "Bilibili";

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == "manga.bilibili.com";
        }
    }
}
