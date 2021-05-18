using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test.Providers
{
    internal class DataProviderComicSourceCondition : ComicSourceConditionBase<DataProvider>
    {
        public DataProviderComicSourceCondition()
        {
        }

        public DataProviderComicSourceCondition(string engineName,Uri address,Uri faviconAddress, Func<ComicSourceContext, bool> conditionAction)
        {
            Address = address;
            ConditionAction = conditionAction;
            FaviconAddress = faviconAddress;
        }

        public override string EngineName { get; }="any";

        public override Uri Address { get; } = new Uri("http://localhost:123");

        public override Uri FaviconAddress { get; }

        public Func<ComicSourceContext,bool> ConditionAction { get; set; }

        public override bool Condition(ComicSourceContext context)
        {
            if (ConditionAction is null)
            {
                return false;
            }
            return ConditionAction(context);
        }
    }
}
