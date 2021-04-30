using Anf.Easy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Anf.KnowEngines;
using System.IO;
using Anf.Easy.Store;
using System;
using Anf.Web.Services;
using Anf.Easy.Visiting;
using Anf.Platform;
using Anf.Engine;
using StackExchange.Redis;

namespace Anf.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppEngine.Reset(services);
            AppEngine.AddServices(NetworkAdapterTypes.HttpClient);
            var store = FileStoreService.FromMd5Default(Path.Combine(Environment.CurrentDirectory, XComicConst.CacheFolderName));

            services.AddSingleton<IComicSaver>(store);
            services.AddSingleton<IStoreService>(store);
            services.AddSingleton<IResourceFactoryCreator<Stream>, PlatformResourceCreatorFactory<Stream>>();
            services.AddSingleton<ProposalEngine>();
            services.AddScoped<IComicVisiting<Stream>, StoreComicVisiting<Stream>>();
            services.AddKnowEngines();
            services.AddControllersWithViews();
            services.AddSingleton<SharedComicVisiting>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
            services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                var config = x.GetRequiredService<IConfiguration>()["ConnectionStrings:CacheConnection"];
                return ConnectionMultiplexer.Connect(config);
            });
            services.AddSignalR()
                .AddAzureSignalR();
            services.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration["ConnectionStrings:CacheConnection"];
            });
            services.AddDbContext<AnfDbContext>((x, y) =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                y.UseSqlServer(config["ConnectionStrings:anfdb"]);
            });
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration["ConnectionStrings:CacheConnection"];
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAzureSignalR(builder =>
            {

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

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            app.ApplicationServices.UseKnowEngines();
        }
    }
}
