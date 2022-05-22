using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddSignalR(IServiceCollection services,IConfiguration configuration)
        {
            services.AddSignalR()
                .AddAzureSignalR(opt =>
                {
                    opt.ConnectionString = configuration["ConnectionStringsSignalr"];
                });
            return this;
        }
    }
}
