using Anf.Engine.Annotations;
using System;

namespace Anf.KnowEngines
{
    [ComicSourceCondition]
    public class MangabzComicCondition : ComicSourceConditionBase<MangabzComicOperator>
    {
        public override string EngineName => "Mangabz";

        public override Uri Address { get; } = new Uri("http://www.mangabz.com/");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
