﻿using System;

namespace Anf.KnowEngines
{
    public class BikabikaComicCondition : ComicSourceConditionBase<BikabikaComicOperator>
    {
        public override string EnginName => "Bikabika";

        public override Uri Address { get; } = new Uri("http://www.bikabika.com/");

        public override Uri FaviconAddress => null;

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
