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
        public JisuComicOperator(IHttpClientFactory clientFactory, IJsEngine v8) : base(clientFactory, v8)
        {
        }
        protected override HttpClient GetHttpClient(IHttpClientFactory clientFactory)
        {
            return clientFactory.CreateClient(ComicConst.EngineJisu);
        }
        protected override string GetBaseAddress()
        {
            return "http://www.1kkk.com/";
        }
    }
}
