using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;
using Kw.Comic.Engine;
using Kw.Comic.Engine.Dm5;
using Kw.Comic.Engine.Dmzj;
using Kw.Comic.Engine.Easy;
using Kw.Comic.Engine.Easy.Visiting;
using Kw.Comic.Engine.Jisu;
using Kw.Comic.Engine.Kuaikan;
using Kw.Comic.Engine.Networks;
using Kw.Comic.Engine.Soman;
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
            services.AddSingleton<IResourceFactoryCreator<Stream>>(StreamResourceFactory.Default);
            services.AddSingleton<IComicVisiting<Stream>, ComicVisiting<Stream>>();
        }
        public static void AddDefaultEasyComic(this IServiceCollection services, NetworkAdapterTypes networkAdapterType = NetworkAdapterTypes.HttpClient)
        {
            services.AddEasyComic(networkAdapterType);
            services.AddStreamVisitor();
        }
        public static void AddEasyComic(this IServiceCollection services, NetworkAdapterTypes networkAdapterType = NetworkAdapterTypes.HttpClient)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton(x =>
            {
                var eng = new ComicEngine
                {
                    new DmzjComicSourceCondition(),
                    new Dm5ComicSourceCondition(),
                    new JisuComicSourceCondition(),
                    new KuaikanComicSourceCondition()
                };
                return eng;
            });
            services.AddSingleton(x =>
            {
                var factory = x.GetRequiredService<IServiceScopeFactory>();
                var eng = new SearchEngine(factory) { typeof(SomanSearchProvider) };
                return eng;
            });
            services.AddSingleton<IComicDownloader, ComicDownloader>();

            services.AddScoped<JisuComicOperator>();
            services.AddScoped<Dm5ComicOperator>();
            services.AddScoped<DmzjComicOperator>();
            services.AddScoped<KuaikanComicOperator>();
            services.AddScoped<SomanSearchProvider>();

            services.AddScoped<IJsEngine, JintJsEngine>();
            services.AddSingleton<RecyclableMemoryStreamManager>();
#if NET45 || NETSTANDARD1_4
            services.AddSingleton<HttpClient>();
            services.AddScoped<INetworkAdapter, HttpClientAdapter>();
#else
            services.AddHttpClient();
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
