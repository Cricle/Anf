using JavaScriptEngineSwitcher.Core;
using Anf.Networks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Anf.Engine.Annotations;

namespace Anf.KnowEngines
{
    [ComicSourceProvider]
    public class JisuComicOperator : Dm5ComicOperator
    {
        public JisuComicOperator(IJsEngine v8,INetworkAdapter networkAdapter)
            : base(v8,networkAdapter)
        {
        }
        protected override string GetBaseAddress()
        {
            return "http://www.1kkk.com/";
        }
    }
}
