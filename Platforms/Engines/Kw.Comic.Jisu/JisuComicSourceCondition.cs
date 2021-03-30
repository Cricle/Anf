using Kw.Comic.Engine;

namespace Kw.Comic.Jisu
{
    public class JisuComicSourceCondition : ComicSourceConditionBase<JisuComicOperator>
    {
        public override string EnginName => ComicConst.EngineJisu;

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == "www.1kkk.com";
        }
    }
}
