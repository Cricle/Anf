using Microsoft.Extensions.DependencyInjection;
using System;
namespace Anf.Test
{
    internal class ValueSceop : IServiceScope
    {

        public IServiceProvider ServiceProvider { get; set; }

        public void Dispose()
        {
        }
    }
}
