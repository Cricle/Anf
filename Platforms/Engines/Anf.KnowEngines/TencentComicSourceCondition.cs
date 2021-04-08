using Anf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.KnowEngines
{
    public class TencentComicSourceCondition : ComicSourceConditionBase<TencentComicOperator>
    {
        public override string EnginName => "Tencent";

        public override bool Condition(ComicSourceContext context)
        {
            return context.Uri.Host == "ac.qq.com";
        }
    }
}
