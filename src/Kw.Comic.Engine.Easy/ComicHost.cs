using System;

namespace Kw.Comic.Engine.Easy
{
    internal class ComicHost : IComicHost
    {
        private readonly IServiceProvider serviceProvider;

        public ComicHost(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }
    }
}
