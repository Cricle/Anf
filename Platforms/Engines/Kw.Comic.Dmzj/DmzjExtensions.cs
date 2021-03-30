using Kw.Comic.Dmzj;
using Kw.Comic.Engine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class DmzjExtensions
    {
        public static void AddDmzjEngine(this IServiceCollection services)
        {
            services.AddScoped<DmzjComicOperator>();
        }
        public static void UseDmzjEngine(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<ComicEngine>();
            eng.Add(new DmzjComicSourceCondition());
        }
    }
}
