﻿using Anf;

namespace Anf.KnowEngines
{
    public class Dm5ComicSourceCondition : ComicSourceConditionBase<Dm5ComicOperator>
    {
        public override string EnginName => ComicConst.EngineDM5;


        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == "www.dm5.com";
        }
    }
}