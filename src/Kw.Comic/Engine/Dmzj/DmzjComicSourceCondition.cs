using Kw.Comic.Engine;

namespace Kw.Comic.Engine.Dmzj
{
    public class DmzjComicSourceCondition : ComicSourceConditionBase<DmzjComicOperator>
    {
        public override string EnginName => ComicConst.EngineDMZJ;

        public override string HttpName => ComicConst.EngineDMZJ;

        public override string ImageHttpName => ComicConst.ImageEngineDMZJ;

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == "www.dmzj.com"||
                context.Uri.Host == "manhua.dmzj.com";
        }
    }
}
