using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
namespace Anf.Test
{
    internal class NullServiceScopeFactory : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            return null;
        }
    }
}
