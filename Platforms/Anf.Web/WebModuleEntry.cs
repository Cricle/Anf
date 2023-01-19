using Structing;
using Structing.Core;
using Structing.Web;
using Anf.Easy;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Anf.KnowEngines;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using Structing.AspNetCore;
using Microsoft.Extensions.Logging;

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
            services.AddLogging(x => x.AddConsole());
            //var store = FileStoreService.FromMd5Default(Path.Combine(Environment.CurrentDirectory, XComicConst.CacheFolderName));
            AddComicAnalysis(services)
                .AddSpa(services)
                .AddFetch(services,config)
                .AddSwagger(services)
                .AddCache(services,config);


#if !DEBUG
            services.AddApplicationInsightsTelemetry(config["APPINSIGHTSCONNECTIONSTRING"]);
#endif

            services.AddControllersWithViews();
            services.AddResponseCaching();
            services.AddResponseCompression();
            services.AddNormalSecurityService();

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
            app.UseResponseCompression();
            if (!picker.IsDevelopment)
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            if (picker.IsDevelopment)
            {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/Anf/swagger.json", "Anf API");
            });
            }
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

                spa.Options.SourcePath = "wwwroot";

                if (picker.IsDevelopment)
                {
                    //spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                    //spa.UseAngularCliServer(npmScript: "start");
                }
            });

            return base.ReadyAsync(context);
        }
    }
}
