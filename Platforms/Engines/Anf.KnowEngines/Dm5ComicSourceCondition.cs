using Anf;

using System;
namespace Anf.KnowEngines
{
    public class Dm5ComicSourceCondition : ComicSourceConditionBase<Dm5ComicOperator>
    {
        public override string EnginName => ComicConst.EngineDM5;
        public override Uri Address { get; } = new Uri("https://www.dm5.com");
        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
