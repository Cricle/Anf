using Microsoft.Extensions.DependencyInjection;

namespace Kw.Comic.Engine.Test
{
    internal class NullServiceScopeFactory : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            return null;
        }
    }
}
