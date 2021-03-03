using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;
using Kw.Comic.Engine;
using Kw.Comic.Engine.Dm5;
using Kw.Comic.Engine.Dmzj;
using Kw.Comic.Engine.Jisu;
using Kw.Comic.Engine.Kuaikan;
using Kw.Comic.Engine.Soman;
using Kw.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic
{
    public class ComicModuleEntry : AutoModuleEntity
    {
        public override void Register(IRegisteContext context)
        {
            WebRequest.DefaultWebProxy = null;

            base.Register(context);

            context.Services.AddScoped<IJsEngine, JintJsEngine>();
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
