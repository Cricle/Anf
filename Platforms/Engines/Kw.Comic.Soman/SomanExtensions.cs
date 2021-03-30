using Kw.Comic.Soman;
using Kw.Comic.Engine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class SomanExtensions
    {
        public static void AddSomanEngine(this IServiceCollection services)
        {
            services.AddScoped<SomanSearchProvider>();
        }
        public static void UseSomanEngine(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<SearchEngine>();
            eng.Add(typeof(SomanSearchProvider));
        }
    }
}
