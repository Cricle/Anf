using Kw.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic
{
    public abstract class AppEngine : IServiceProvider
    {
        private readonly IServiceCollection services;
        private readonly ModuleCollection modules = new ModuleCollection();
        private bool loaded;

        private IServiceProvider serviceProvider;

        protected AppEngine(IServiceCollection services)
        {
            this.services = services;
        }

        public bool Loaded => loaded;

        public IServiceProvider ServiceProvider => serviceProvider;

        public IServiceCollection Services => services;

        public ModuleCollection Modules => modules;

        public void Load()
        {
            if (loaded)
            {
                return;
            }
            loaded = true;
            modules.Add(new ComicModuleEntry());
            var ctx = new RegisteContext(services);
            modules.ReadyRegister(ctx);
            modules.Register(ctx);
            serviceProvider = BuildProvider();
        }
        protected abstract IServiceProvider BuildProvider();
        public async Task ReadyAsync()
        {
            var ctx = new ReadyContext(ServiceProvider);
            await modules.BeforeReadyAsync(ctx);
            await modules.ReadyAsync(ctx);
            await modules.AfterReadyAsync(ctx);
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetRequiredService(serviceType);
        }
        public IServiceScope GetScope()
        {
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            return serviceScopeFactory.CreateScope();
        }
    }
}
