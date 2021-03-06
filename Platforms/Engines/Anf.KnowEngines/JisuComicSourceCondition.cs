using Anf;
using Anf.Engine.Annotations;
using System;

namespace Anf.KnowEngines
{
    [ComicSourceCondition]
    public class JisuComicSourceCondition : ComicSourceConditionBase<JisuComicOperator>
    {
        public override string EngineName => ComicConst.EngineJisu;

        public override Uri Address { get; } = new Uri("https://www.1kkk.com");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
