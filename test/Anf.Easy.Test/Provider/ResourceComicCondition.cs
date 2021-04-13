namespace Anf.Easy.Test.Provider
{
    internal class ResourceComicCondition : ComicSourceConditionBase<ResourceComicProvider>
    {
        public override string EnginName => "Resource";

        public override bool Condition(ComicSourceContext context)
        {
            return true;
        }
    }
}
