using Microsoft.Extensions.DependencyInjection;
using System;

namespace Anf.Easy.Test
{
    internal class NullServiceScope : IServiceScope
    {
        public IServiceProvider ServiceProvider => null;

        public void Dispose()
        {
        }

    }
}
