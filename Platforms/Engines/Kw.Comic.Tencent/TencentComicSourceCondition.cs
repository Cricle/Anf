using Kw.Comic.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Tencent
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
