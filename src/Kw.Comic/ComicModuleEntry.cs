using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;
using Kw.Comic.Engine;
using Kw.Comic.Engine.Dm5;
using Kw.Comic.Engine.Dmzj;
using Kw.Comic.Engine.Jisu;
using Kw.Comic.Engine.Kuaikan;
using Kw.Comic.Engine.Networks;
using Kw.Comic.Engine.Soman;
using Kw.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic
{
    public class ComicModuleEntry : AutoModuleEntity
    {
        public override void Register(IRegisteContext context)
        {
            WebRequest.DefaultCachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            WebRequest.DefaultWebProxy = null;

            base.Register(context);

            context.Services.AddSingleton<ComicEngine>();
            context.Services.AddSingleton<SearchEngine>();

            context.Services.AddScoped<JisuComicOperator>();
            context.Services.AddScoped<Dm5ComicOperator>();
            context.Services.AddScoped<DmzjComicOperator>();
            context.Services.AddScoped<KuaikanComicOperator>();
            context.Services.AddScoped<SomanSearchProvider>();

            context.Services.AddScoped<IJsEngine, JintJsEngine>();
#if NET452
            context.Services.AddSingleton<HttpClient>();
#else
            context.Services.AddHttpClient();
#endif
            context.Services.AddScoped<INetworkAdapter, HttpClientAdapter>();
        }
        public override Task ReadyAsync(IReadyContext context)
        {
            var eng = context.GetRequiredService<ComicEngine>();
            eng.Add(new DmzjComicSourceCondition());
            eng.Add(new Dm5ComicSourceCondition());
            eng.Add(new JisuComicSourceCondition());
            //eng.Add(new KuaikanComicSourceCondition());

            var searchEng = context.GetRequiredService<SearchEngine>();
            searchEng.Add(typeof(SomanSearchProvider));
            return base.ReadyAsync(context);
        }
    }
}
