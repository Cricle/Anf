using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Kw.Comic.KnowEngines
{
    public static class KnowEnginesExtensions
    {
        public static void AddKnowEngines(this IServiceCollection services)
        {
            services.AddDm5Engine();
            services.AddDmzjEngine();
            services.AddKuaikanEngine();
            services.AddJisuEngine();
            services.AddSomanEngine();
            services.AddTencentEngine();
        }
        public static void UseKnowEngines(this IServiceProvider provider)
        {
            provider.UseDm5Engine();
            provider.UseDmzjEngine();
            provider.UseJisuEngine();
            provider.UseKuaikanEngine();
            provider.UseSomanEngine();
            provider.UseTencentEngine();
        }
    }
}
