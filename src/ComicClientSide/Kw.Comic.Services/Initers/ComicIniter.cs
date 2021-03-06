using Kw.Comic.Engine;
using Kw.Comic.Engine.Dm5;
using Kw.Comic.Engine.Dmzj;
using Kw.Comic.Engine.Jisu;
using Kw.Comic.Engine.Kuaikan;
using Kw.Comic.Engine.Soman;
using Kw.Core;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Services.Initers
{
    [ModuleIniter]
    internal class ComicIniter : IModuleInit
    {
        public Task InvokeAsync(IReadyContext context)
        {
            var eng = context.GetRequiredService<ComicEngine>();
            eng.Add(new DmzjComicSourceCondition());
            eng.Add(new Dm5ComicSourceCondition());
            eng.Add(new JisuComicSourceCondition());
            eng.Add(new KuaikanComicSourceCondition());

            var searchEng = context.GetRequiredService<SearchEngine>();
            searchEng.Add(typeof(SomanSearchProvider));

            return Task.CompletedTask;
        }
    }
}
