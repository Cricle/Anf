using Kw.Comic.Blazor.Server.Services;
using Kw.Comic.Engine;
using Kw.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kw.Comic.Blazor.Server
{
    internal class AppModuleEntry : AutoModuleEntity
    {
        public override void Register(IRegisteContext context)
        {
            var cfg = context.GetConfiguaration();
            context.Services.AddGrpc(x =>
            {
            });
            context.Services.AddControllersWithViews()
                .AddJsonOptions(x=> 
                {
                    x.JsonSerializerOptions.PropertyNamingPolicy = null;
                });
            context.Services.AddRazorPages();
            context.Services.AddMemoryCache();
            var redisConfig = cfg["Redis:Connection"];
            var redisConn = ConnectionMultiplexer.Connect(redisConfig);
            context.Services.AddSingleton(redisConn);
            context.Services.AddSingleton(s =>
            {
                var ser1 = s.GetRequiredService<ComicEngine>();
                var scopeFactory = s.GetRequiredService<IServiceScopeFactory>();
                var opt = s.GetRequiredService<IOptions<AnalysisOptions>>();
                var conn = s.GetRequiredService<ConnectionMultiplexer>();
                var anaSer = new AnalysisService(ser1, scopeFactory, opt,conn);
                anaSer.Begin();
                return anaSer;
            });
            context.Services.AddOptions<AnalysisOptions>("AnalysisOptions");
            base.Register(context);
        }
    }
}
