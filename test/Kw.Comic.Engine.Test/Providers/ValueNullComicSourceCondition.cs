using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test.Providers
{
    internal class DataProviderComicSourceCondition : ComicSourceConditionBase<DataProvider>
    {
        public override string EnginName => "any";

        public override bool Condition(ComicSourceContext context)
        {
            return false;
        }
    }
}
