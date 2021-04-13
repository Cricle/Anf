using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test
{
    internal class ValueServiceScopeFactory : IServiceScopeFactory
    {

        public Func<IServiceScope> ScopeFactory { get; set; }
        public IServiceScope CreateScope()
        {
            return ScopeFactory();
        }
    }
}
