using System;

namespace Anf.KnowEngines
{
    public class MangabzComicCondition : ComicSourceConditionBase<MangabzComicOperator>
    {
        public override string EnginName => "Mangabz";

        public override Uri Address { get; } = new Uri("http://www.mangabz.com/");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
