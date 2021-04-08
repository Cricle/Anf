using System;
using System.Collections.Generic;
using System.Text;
using Anf;
using Microsoft.Extensions.DependencyInjection;

namespace Anf.KnowEngines
{
    public static class KnowEnginesExtensions
    {
        public static void AddKnowEngines(this IServiceCollection services)
        {
            services.AddScoped<Dm5ComicOperator>();
            services.AddScoped<DmzjComicOperator>();
            services.AddScoped<KuaikanComicOperator>();
            services.AddScoped<JisuComicOperator>();
            services.AddScoped<SomanSearchProvider>();
            services.AddScoped<TencentComicOperator>();
        }
        public static void UseKnowEngines(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<ComicEngine>();
            eng.Add(new Dm5ComicSourceCondition());
            eng.Add(new DmzjComicSourceCondition());
            eng.Add(new JisuComicSourceCondition());
            eng.Add(new KuaikanComicSourceCondition());
            eng.Add(new TencentComicSourceCondition());
            var searchEng = provider.GetRequiredService<SearchEngine>();
            searchEng.Add(typeof(SomanSearchProvider));
        }
    }
}
