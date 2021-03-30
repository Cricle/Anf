using Kw.Comic.Jisu;
using Kw.Comic.Engine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class JisuExtensions
    {
        public static void AddJisuEngine(this IServiceCollection services)
        {
            services.AddScoped<JisuComicOperator>();
        }
        public static void UseJisuEngine(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<ComicEngine>();
            eng.Add(new JisuComicSourceCondition());
        }
    }
}
