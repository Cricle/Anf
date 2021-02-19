using Kw.Core;
using Microsoft.Extensions.Configuration;
using System;

namespace Kw.Comic.Blazor.Server
{
    internal class ReadyContext : IReadyContext
    {
        public ReadyContext(IServiceProvider provider, IConfiguration configuration)
        {
            Provider = provider;
            Configuration = configuration;
        }

        public IServiceProvider Provider { get; }

        public IConfiguration Configuration { get; }

        public IKwEnvironment KwEnviroment => null;

        public object GetService(Type serviceType)
        {
            return Provider.GetService(serviceType);
        }
    }
}
