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
            services.AddScoped<BilibiliComicOperator>();
            services.AddScoped<QimiaoComicOperator>();
            services.AddScoped<MangabzComicOperator>();
            services.AddScoped<XmanhuaComicOperator>();
        }
        public static void UseKnowEngines(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<ComicEngine>();
            eng.Add(new Dm5ComicSourceCondition());
            eng.Add(new DmzjComicSourceCondition());
            eng.Add(new JisuComicSourceCondition());
            eng.Add(new KuaikanComicSourceCondition());
            eng.Add(new TencentComicSourceCondition());
            eng.Add(new BilibiliComicSourceCondition());
            eng.Add(new QimianComicSourceCondition());
            eng.Add(new MangabzComicCondition());
            eng.Add(new XmanhuaComicCondition());
            var searchEng = provider.GetRequiredService<SearchEngine>();
            searchEng.Add(typeof(SomanSearchProvider));
        }
    }
}
