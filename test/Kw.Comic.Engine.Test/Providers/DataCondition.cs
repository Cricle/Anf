using System;

namespace Anf.Test.Providers
{
    internal class DataCondition : IComicSourceCondition
    {
        public string EnginName { get; set; }

        public int Order { get; set; }

        public EngineDescript Descript { get; set; }

        public Type ProviderType { get; set; }

        public Func<ComicSourceContext,bool> ConditionFunc { get; set; }

        public bool Condition(ComicSourceContext context)
        {
            return ConditionFunc(context);
        }
    }
}
