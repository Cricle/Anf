using Kw.Comic.Blazor.Server.Rpc;
using Kw.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Blazor.Server
{
    public class Startup
    {

        internal static readonly IModuleEntry[] modules = new IModuleEntry[]
        {
            new AppModuleEntry(),
            new ComicModuleEntry()
        };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var ctx = new RegisteContext(services);
            ctx.SetConfiguaration(Configuration);
            foreach (var item in modules)
            {
                item.ReadyRegister(ctx);
            }
            foreach (var item in modules)
            {
                item.Register(ctx);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var ctx = new ReadyContext(app.ApplicationServices,Configuration);

            async Task ReadyModule()
            {
                foreach (var item in modules)
                {
                    await item.BeforeReadyAsync(ctx);
                }
                foreach (var item in modules)
                {
                    await item.ReadyAsync(ctx);
                }
                foreach (var item in modules)
                {
                    await item.AfterReadyAsync(ctx);
                }
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            ReadyModule().GetAwaiter().GetResult();
            //app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseGrpcWeb();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
                endpoints.MapGrpcService<ComicAnalysisingService>()
                    .EnableGrpcWeb();
            });
        }
    }
}
