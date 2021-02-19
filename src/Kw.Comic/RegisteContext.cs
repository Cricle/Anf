using Kw.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;

namespace Kw.Comic
{
    internal class RegisteContext : IRegisteContext
    {
        public RegisteContext(IServiceCollection services)
        {
            Services = services;
            Features = new Dictionary<object, object>();
        }

        public IServiceCollection Services { get; }

        public IDictionary Features { get; }
    }
}
