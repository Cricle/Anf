using Microsoft.Extensions.DependencyInjection;

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
