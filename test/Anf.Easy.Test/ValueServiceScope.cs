using Microsoft.Extensions.DependencyInjection;
using System;

namespace Anf.Easy.Test
{
    internal class ValueServiceScope : IServiceScope
    {
        public IServiceProvider ServiceProvider { get; set; }

        public void Dispose()
        {
        }
    }
}
