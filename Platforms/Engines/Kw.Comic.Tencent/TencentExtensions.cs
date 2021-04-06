using Kw.Comic.Engine;
using Kw.Comic.Tencent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TencentExtensions
    {
        public static void AddTencentEngine(this IServiceCollection services)
        {
            services.AddScoped<TencentComicOperator>();
        }
        public static void UseTencentEngine(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<ComicEngine>();
            eng.Add(new TencentComicSourceCondition());
        }
    }
}
