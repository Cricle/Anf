using Kw.Comic.Kuaikan;
using Kw.Comic.Engine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class KuaikanExtensions
    {
        public static void AddKuaikanEngine(this IServiceCollection services)
        {
            services.AddScoped<KuaikanComicOperator>();
        }
        public static void UseKuaikanEngine(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<ComicEngine>();
            eng.Add(new KuaikanComicSourceCondition());
        }
    }
}
