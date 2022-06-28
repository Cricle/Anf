using Structing;
using Structing.Core;
using Structing.Web;
using Anf.Easy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Anf.KnowEngines;
using System.IO;
using Anf.Easy.Store;
using System;
using Anf.Easy.Visiting;
using Anf.Engine;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Anf.ResourceFetcher;
using Anf.ChannelModel.Entity;
using Anf.ResourceFetcher.Fetchers;
using Anf.WebService;
using Anf.ResourceFetcher.Redis;
using Quartz.Impl;
using System.Threading.Tasks;
using Quartz;
using System.Linq;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using Anf.Statistical;
using System.Collections;
using Structing.AspNetCore;
using Ao.Cache.Redis.Converters;
using Ao.Cache.Redis;
using System.Text.Json;

namespace Anf.Web
{
    [EnableApplicationPart]
    internal partial class WebModuleEntry : AutoModuleEntry
    {
        public override void Register(IRegisteContext context)
        {
            base.Register(context);

            var services = context.Services;
            var config = context.Features.GetConfiguration();

            services.AddSingleton(Program.modules);

            //var store = FileStoreService.FromMd5Default(Path.Combine(Environment.CurrentDirectory, XComicConst.CacheFolderName));
            AddComicAnalysis(services)
                .AddAzureStore(services,config)
                .AddSpa(services)
                .AddSignalR(services, config)
                .AddEF(services, config)
                .AddCache(services, config)
                .AddFetch(services,config)
                .AddSwagger(services)
                .AddQuartz(services)
                .AddAuth(services);


#if !DEBUG
            services.AddApplicationInsightsTelemetry(config["APPINSIGHTSCONNECTIONSTRING"]);
#endif

            services.AddControllersWithViews();
            services.AddResponseCaching();
            services.AddResponseCompression();
            services.AddNormalSecurityService();


            services.AddOptions<ResourceLockOptions>();

            KnowsCacheValueConverter.EndValueConverter = new JsonCacheValueConverter();

        }
        public override async Task AfterReadyAsync(IReadyContext context)
        {
            context.UseKnowEngines();
            var eng = context.GetComicEngine();
            var tx = eng.FirstOrDefault(x => x is TencentComicSourceCondition);
            if (tx != null)
            {
                eng.Remove(tx);
            }
            using (var s = context.GetServiceScope())
            {
                var db = s.ServiceProvider.GetRequiredService<AnfDbContext>();
                db.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));
                db.Database.EnsureCreated();
            }
            await base.AfterReadyAsync(context);
        }
        public override Task ReadyAsync(IReadyContext context)
        {
            var app = context.Features.GetApplicationBuilder();
            var picker = context.Features.GetServicePicker();
            if (picker.IsDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
#if !DEBUG
            app.UseHttpsRedirection();
#endif
            app.UseResponseCaching();
            app.UseResponseCompression();
            if (!picker.IsDevelopment)
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAzureSignalR(builder =>
            {
                //builder.MapHub<ReadingHub>("/hubs/v1/reading");
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/Anf/swagger.json", "Anf API");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp/dist";

                if (picker.IsDevelopment)
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                    //spa.UseAngularCliServer(npmScript: "start");
                }
            });

            return base.ReadyAsync(context);
        }
    }
    internal class JsonCacheValueConverter : ICacheValueConverter
    {
        public RedisValue Convert(object instance, object value, ICacheColumn column)
        {
            return JsonSerializer.Serialize(value);
        }

        public object ConvertBack(in RedisValue value, ICacheColumn column)
        {
            return JsonSerializer.Deserialize(value, column.Property.PropertyType);
        }
    }
}
