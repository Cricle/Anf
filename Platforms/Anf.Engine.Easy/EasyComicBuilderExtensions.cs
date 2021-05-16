using Anf;
using Anf.Easy;
using Anf.Easy.Visiting;
using Anf.Networks;
using Microsoft.IO;
using System;
using System.IO;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EasyComicBuilderExtensions
    {
        public static void AddStreamVisitor(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IResourceFactoryCreator<Stream>>(StreamResourceFactoryCreator.Default);
            services.AddSingleton<IComicVisiting<Stream>, ComicVisiting<Stream>>();
        }
        public static void AddDefaultEasyComic(this IServiceCollection services, NetworkAdapterTypes networkAdapterType = NetworkAdapterTypes.HttpClient)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddEasyComic(networkAdapterType);
            services.AddStreamVisitor();
        }
        public static void AddEasyComic(this IServiceCollection services, NetworkAdapterTypes networkAdapterType = NetworkAdapterTypes.HttpClient)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<ComicEngine>();
            services.AddSingleton(x =>
            {
                var factory = x.GetRequiredService<IServiceScopeFactory>();
                var eng = new SearchEngine(factory);
                return eng;
            });
            services.AddSingleton<IComicDownloader, ComicDownloader>();

            services.AddSingleton<RecyclableMemoryStreamManager>();
#if NET45 || NETSTANDARD1_4
            services.AddSingleton<HttpClient>();
            services.AddScoped<INetworkAdapter, HttpClientAdapter>();
#else
            services.AddHttpClient();
            services.AddScoped(x => x.GetRequiredService<IHttpClientFactory>().CreateClient());
            if (networkAdapterType == NetworkAdapterTypes.HttpClient)
            {
                services.AddScoped<INetworkAdapter, HttpClientAdapter>();
            }
            else
            {
                services.AddScoped<INetworkAdapter, WebRequestAdapter>();
            }
#endif
        }
    }
}
