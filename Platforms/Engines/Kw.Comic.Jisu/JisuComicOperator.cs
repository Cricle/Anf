using JavaScriptEngineSwitcher.Core;
using Kw.Comic.Dm5;
using Kw.Comic.Engine.Networks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Kw.Comic.Jisu
{
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
