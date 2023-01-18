using Microsoft.Extensions.DependencyInjection;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddSpa(IServiceCollection services)
        {
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });
            return this;
        }
    }
}
