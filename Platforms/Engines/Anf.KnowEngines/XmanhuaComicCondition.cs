using System;

namespace Anf.KnowEngines
{
    public class XmanhuaComicCondition : ComicSourceConditionBase<MangabzComicOperator>
    {
        public override string EnginName => "Xmanhua";

        public override Uri Address { get; } = new Uri("http://www.xmanhua.com/");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
