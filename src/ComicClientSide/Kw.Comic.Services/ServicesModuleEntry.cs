using Kw.Comic.Services;
using Kw.Core;
using Kw.Core.Annotations;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Kw.Comic.Services.Options;
using Kw.Comic.Engine.Dm5;
using Kw.Comic.Engine.Dmzj;
using Kw.Comic.Engine.Jisu;
using Kw.Comic.Engine.Kuaikan;
using Kw.Comic.Engine.Networks;
using Kw.Comic.Engine.Soman;
using Kw.Comic.Engine;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;

[assembly:KwModuleEntry(typeof(ServicesModuleEntry))]

namespace Kw.Comic.Services
{
    public class ServicesModuleEntry : AutoModuleEntity
    {
        public override void Register(IRegisteContext context)
        {
            base.Register(context);
            var option = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
            };
            context.Services.AddSingleton<IConnectionMultiplexer>(ser =>
            {
                var opt = ser.GetRequiredService<IOptions<RedisOptions>>();
                var redisOpt = new ConfigurationOptions
                {
                     AbortOnConnectFail=opt.Value.AbortOnConnectFail,
                     KeepAlive=opt.Value.KeepAlive
                };
                foreach (var item in opt.Value.Endpoints)
                {
                    redisOpt.EndPoints.Add(item);
                }
                return ConnectionMultiplexer.Connect(redisOpt);
            });

            context.Services.AddSingleton<ComicEngine>();
            context.Services.AddSingleton<SearchEngine>();

            context.Services.AddScoped<JisuComicOperator>();
            context.Services.AddScoped<Dm5ComicOperator>();
            context.Services.AddScoped<DmzjComicOperator>();
            context.Services.AddScoped<KuaikanComicOperator>();
            context.Services.AddScoped<SomanSearchProvider>();

            context.Services.AddScoped<IJsEngine, JintJsEngine>();
            context.Services.AddScoped<INetworkAdapter, WebRequestAdapter>();

        }
    }
}
