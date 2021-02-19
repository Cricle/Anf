namespace Kw.Comic.Engine.Dm5
{
    public class Dm5ComicSourceCondition : ComicSourceConditionBase<Dm5ComicOperator>
    {
        public override string EnginName => ComicConst.EngineDM5;

        public override string HttpName => ComicConst.EngineDM5;

        public override string ImageHttpName => ComicConst.EngineDM5;


        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == "www.dm5.com";
        }
    }
}
