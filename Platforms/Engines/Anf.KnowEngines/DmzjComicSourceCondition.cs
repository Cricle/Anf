using Anf;

namespace Anf.KnowEngines
{
    public class DmzjComicSourceCondition : ComicSourceConditionBase<DmzjComicOperator>
    {
        public override string EnginName => ComicConst.EngineDMZJ;

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == "www.dmzj.com"||
                context.Uri.Host == "manhua.dmzj.com";
        }
    }
}
