using Anf.ChannelModel.Entity;
using Anf.WebService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Anf.ResourceFetcher.Fetchers;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddEF(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AnfDbContext>((x, y) =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                y.UseSqlServer(config["ConnectionStringsAnfdb"]);
            },optionsLifetime: ServiceLifetime.Singleton)
            .AddDbContextPool<AnfDbContext>((x, y) =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                y.UseSqlServer(config["ConnectionStringsAnfdb"]);
            }).AddIdentity<AnfUser, AnfRole>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
             .AddEntityFrameworkStores<AnfDbContext>();
            services.AddScoped<IDbContextTransfer, AnfDbContextTransfer>();
            return this;
        }
    }
}
