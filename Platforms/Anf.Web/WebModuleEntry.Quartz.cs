using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddQuartz(IServiceCollection services)
        {
            services.AddQuartz(x =>
            {
                x.UseMicrosoftDependencyInjectionJobFactory();
            });
            services.AddQuartzServer();
            return this;
        }
    }
}
