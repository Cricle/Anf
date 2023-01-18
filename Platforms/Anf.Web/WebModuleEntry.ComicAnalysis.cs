using Anf.Easy;
using Anf.Easy.Visiting;
using Anf.Engine;
using Anf.KnowEngines;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddComicAnalysis(IServiceCollection services)
        {
            AppEngine.Reset(services);
            AppEngine.AddServices(NetworkAdapterTypes.HttpClient);
            services.AddSingleton<ProposalEngine>();
            services.AddScoped<IComicVisiting<Stream>, WebComicVisiting>();
            services.AddKnowEngines();
            return this;
        }
    }
}
