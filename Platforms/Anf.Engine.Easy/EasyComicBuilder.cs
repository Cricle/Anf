using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy
{
    public class EasyComicBuilder
    {
        private static readonly Lazy<IServiceProvider> @default = new Lazy<IServiceProvider>(MakeDefault);

        public static IServiceProvider Default => @default.Value;

        public EasyComicBuilder(IServiceCollection services = null)
        {
            Services = services ?? new ServiceCollection();
            NetworkAdapterType = NetworkAdapterTypes.HttpClient;
        }

        public NetworkAdapterTypes NetworkAdapterType { get; set; }

        public IServiceCollection Services { get; }

        public void AddComicServices()
        {
            Services.AddEasyComic(NetworkAdapterType);
        }
        public IServiceProvider Build()
        {
            var provider = Services.BuildServiceProvider();
            return provider;
        }
        private static IServiceProvider MakeDefault()
        {
            var builder = new EasyComicBuilder();
            builder.AddComicServices();
            return builder.Build();
        }
    }
}
