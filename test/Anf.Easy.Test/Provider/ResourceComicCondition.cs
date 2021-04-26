using System;

namespace Anf.Easy.Test.Provider
{
    internal class ResourceComicCondition : ComicSourceConditionBase<ResourceComicProvider>
    {
        public override string EngineName => "Resource";

        public override Uri Address => new Uri("http://localhost:5213");

        public override bool Condition(ComicSourceContext context)
        {
            return true;
        }
    }
}
