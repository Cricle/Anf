using Anf.Engine.Annotations;
using System;

namespace Anf.KnowEngines
{
    [ComicSourceCondition]
    public class QimianComicSourceCondition : ComicSourceConditionBase<QimiaoComicOperator>
    {
        public override string EngineName => "qimiaomh";

        public override Uri Address { get; } = new Uri("https://www.qimiaomh.com");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
