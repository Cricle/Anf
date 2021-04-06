using Kw.Comic.Engine;

namespace Kw.Comic
{
    public class KuaikanComicSourceCondition : ComicSourceConditionBase<KuaikanComicOperator>
    {
        public override string EnginName => ComicConst.EngineKuaiKan;

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == "www.kuaikanmanhua.com";
        }
    }
}
