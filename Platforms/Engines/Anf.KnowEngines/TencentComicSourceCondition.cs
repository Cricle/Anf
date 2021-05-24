using Anf;
using Anf.Engine.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.KnowEngines
{
    [ComicSourceCondition]
    public class TencentComicSourceCondition : ComicSourceConditionBase<TencentComicOperator>
    {
        public override string EngineName => "Tencent";

        public override Uri Address { get; } = new Uri("https://ac.qq.com");

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == Address.Host;
        }
    }
}
