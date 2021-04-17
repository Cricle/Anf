using System;

namespace Anf.KnowEngines
{
    public class QimianComicSourceCondition : ComicSourceConditionBase<QimiaoComicOperator>
    {
        public override string EnginName => "qimiaomh";

        public override Uri Address { get; } = new Uri("https://www.qimiaomh.com");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
