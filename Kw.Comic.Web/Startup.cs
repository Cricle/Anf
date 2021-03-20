using Kw.Comic.Engine.Easy;
using KwC.Hubs;
using KwC.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using CompressedStaticFiles;
#if !MiniService
using Microsoft.OpenApi.Models;
#endif

namespace KwC
{
    public class Startup
    {
        private static DownloadManager downloadManager;

        public static DownloadManager DownloadManager => downloadManager;

        public static event Action InitDone;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var easyComicBuilder = new EasyComicBuilder(services);
            easyComicBuilder.AddComicServices();

            services.AddSingleton<StoreResourceCreatorFactory>();

            services.AddSingleton<ComicHubVisitor>();
            services.AddControllersWithViews();
            services.AddSingleton<IStoreService>(PhysicalStoreService.FromDefault());
            services.AddSingleton(x => new VisitService(x));
            services.AddCompressedStaticFiles();
            services.AddResponseCompression(x=>
            {
                x.Providers.Add<GzipCompressionProvider>();
                x.Providers.Add<BrotliCompressionProvider>();
            });
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddSignalR().AddJsonProtocol();

#if !MiniService
            services.AddMiniProfiler(options => 
            {
                options.RouteBasePath = "/profiler";
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Comic-API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
#endif

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

            downloadManager = new DownloadManager(app.ApplicationServices);

            app.UseResponseCompression();
#if !MiniService
            app.UseMiniProfiler();
#endif
            app.UseHttpsRedirection();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseSwaggerUI();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ComicHub>("/api/hubs/comic");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
#if !MiniService
                endpoints.MapSwagger();
#endif
            });

            app.UseCompressedStaticFiles();
            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200/");
                }
            });
            InitDone?.Invoke();
        }
    }
}
