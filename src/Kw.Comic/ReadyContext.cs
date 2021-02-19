using Kw.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kw.Comic
{
    internal class ReadyContext : IReadyContext
    {
        public ReadyContext(IServiceProvider provider)
        {
            Provider = provider;
        }

        public IServiceProvider Provider { get; }

        public IConfiguration Configuration => Provider.GetService<IConfiguration>();

        public IKwEnvironment KwEnviroment => Provider.GetService<IKwEnvironment>();

        public object GetService(Type serviceType)
        {
            return Provider.GetService(serviceType);
        }
    }
}
