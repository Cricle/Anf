using JavaScriptEngineSwitcher.Core;
using Kw.Comic.Engine.Dm5;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Kw.Comic.Engine.Jisu
{
    [EnableService(ServiceLifetime = ServiceLifetime.Scoped)]
    public class JisuComicOperator : Dm5ComicOperator
    {
        public JisuComicOperator(IJsEngine v8) : base(v8)
        {
        }
        protected override string GetBaseAddress()
        {
            return "http://www.1kkk.com/";
        }
    }
}
