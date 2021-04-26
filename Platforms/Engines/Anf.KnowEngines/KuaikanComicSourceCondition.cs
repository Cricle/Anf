using Anf;
using System;

namespace Anf.KnowEngines
{
    public class KuaikanComicSourceCondition : ComicSourceConditionBase<KuaikanComicOperator>
    {
        public override string EngineName => ComicConst.EngineKuaiKan;

        public override Uri Address { get; } = new Uri("https://www.kuaikanmanhua.com");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
