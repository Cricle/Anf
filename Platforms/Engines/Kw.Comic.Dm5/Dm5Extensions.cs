using Kw.Comic.Dm5;
using Kw.Comic.Engine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class Dm5Extensions
    {
        public static void AddDm5Engine(this IServiceCollection services)
        {
            services.AddScoped<Dm5ComicOperator>();
        }
        public static void UseDm5Engine(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<ComicEngine>();
            eng.Add(new Dm5ComicSourceCondition());
        }
    }
}
