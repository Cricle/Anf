using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
namespace Anf.Test
{
    internal class ValueServiceScopeFactory : IServiceScopeFactory
    {
        public Dictionary<Type, Func<object>> Factory { get; set; }

        public IServiceScope CreateScope()
        {
            return new ValueSceop { ServiceProvider = new ValueServiceProvider { Factory=Factory} };
        }
    }
}
