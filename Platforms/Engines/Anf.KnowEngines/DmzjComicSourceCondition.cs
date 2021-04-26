using Anf;
using System;

namespace Anf.KnowEngines
{
    public class DmzjComicSourceCondition : ComicSourceConditionBase<DmzjComicOperator>
    {
        public override string EngineName => ComicConst.EngineDMZJ;

        public override Uri Address { get; } = new Uri("https://www.dmzj.com");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host||
                context.Uri.Host == "manhua.dmzj.com";
        }
    }
}
