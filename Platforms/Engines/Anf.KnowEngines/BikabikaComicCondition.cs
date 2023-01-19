using Anf.Engine.Annotations;
using System;

namespace Anf.KnowEngines
{
    [ComicSourceCondition]
    public class BikabikaComicCondition : ComicSourceConditionBase<BikabikaComicOperator>
    {
        public override string EngineName => "Bikabika";

        public override Uri Address { get; } = new Uri("http://www.bikabika.com/");

        public override Uri FaviconAddress => null;

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
