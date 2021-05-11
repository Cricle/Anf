using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anf;
using Anf.Engine;
using Anf.KnowEngines.ProposalProviders;
using Anf.KnowEngines.SearchProviders;
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
            services.AddScoped<TencentComicOperator>();
            services.AddScoped<BilibiliComicOperator>();
            services.AddScoped<QimiaoComicOperator>();
            services.AddScoped<MangabzComicOperator>();
            services.AddScoped<XmanhuaComicOperator>();
            services.AddScoped<BikabikaComicOperator>();

            services.AddScoped<SomanSearchProvider>();
            services.AddScoped<Dm5SearchProvider>();
            services.AddScoped<BilibiliSearchProvider>();
            services.AddScoped<KuaikanSearchProvider>();

            services.AddScoped<Dm5ProposalProvider>();
            services.AddScoped<BilibiliProposalProvider>();
        }
        public static void AddComicSource(this ComicEngine eng)
        {
            eng.Add(new Dm5ComicSourceCondition());
            eng.Add(new DmzjComicSourceCondition());
            eng.Add(new JisuComicSourceCondition());
            eng.Add(new KuaikanComicSourceCondition());
            eng.Add(new TencentComicSourceCondition());
            eng.Add(new BilibiliComicSourceCondition());
            eng.Add(new QimianComicSourceCondition());
            eng.Add(new MangabzComicCondition());
            eng.Add(new XmanhuaComicCondition());
            eng.Add(new BikabikaComicCondition());
        }
        public static void AddSearchProvider(this SearchEngine searchEng)
        {
            searchEng.Add(typeof(SomanSearchProvider));
            searchEng.Add(typeof(Dm5SearchProvider));
            searchEng.Add(typeof(BilibiliSearchProvider));
            searchEng.Add(typeof(KuaikanSearchProvider));
        }
        public static void AddProposalEngine(this ProposalEngine proEng)
        {
            proEng.Add(new Dm5ProposalDescrition());
            proEng.Add(new BilibiliProposalDescription());
        }
        public static void UseKnowEngines(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<ComicEngine>();
            AddComicSource(eng);
            var searchEng = provider.GetRequiredService<SearchEngine>();
            AddSearchProvider(searchEng);
            var proEng = provider.GetRequiredService<ProposalEngine>();
            AddProposalEngine(proEng);
        }
    }
}
