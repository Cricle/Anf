using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;
using Kw.Comic.Engine.Dm5;
using Kw.Comic.Engine.Dmzj;
using Kw.Comic.Engine.Jisu;
using Kw.Comic.Engine.Kuaikan;
using Kw.Comic.Engine.Networks;
using Kw.Comic.Engine.Soman;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    public class EasyComicBuilder
    {
        private static readonly Lazy<IComicHost> @default=new Lazy<IComicHost>(MakeDefault);

        public static IComicHost Default => @default.Value;

        public EasyComicBuilder()
        {
            Services = new ServiceCollection();
            NetworkAdapterType = NetworkAdapterTypes.HttpClient;
        }

        public NetworkAdapterTypes NetworkAdapterType { get; set; }

        public IServiceCollection Services { get; }

        public void AddComicServices()
        {
            Services.AddSingleton(x=> 
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
            Services.AddSingleton(x=>
            {
                var factory=x.GetRequiredService<IServiceScopeFactory>();
                var eng = new SearchEngine(factory) { typeof(SomanSearchProvider) };
                return eng;
            });
            Services.AddSingleton<IComicDownloader,ComicDownloader>();

            Services.AddScoped<JisuComicOperator>();
            Services.AddScoped<Dm5ComicOperator>();
            Services.AddScoped<DmzjComicOperator>();
            Services.AddScoped<KuaikanComicOperator>();
            Services.AddScoped<SomanSearchProvider>();

            Services.AddScoped<IJsEngine, JintJsEngine>();
            Services.AddSingleton<RecyclableMemoryStreamManager>();
#if NET45||NETSTANDARD1_4
            Services.AddSingleton<HttpClient>();
            Services.AddScoped<INetworkAdapter, HttpClientAdapter>();
#else
            Services.AddHttpClient();
            if (NetworkAdapterType == NetworkAdapterTypes.HttpClient)
            {
                Services.AddScoped<INetworkAdapter, HttpClientAdapter>();
            }
            else
            {
                Services.AddScoped<INetworkAdapter, WebRequestAdapter>();
            }
#endif
        }
        public IComicHost Build()
        {
            var provider = Services.BuildServiceProvider();
            var host= new ComicHost(provider);
            return host;
        }
        private static IComicHost MakeDefault()
        {
            var builder = new EasyComicBuilder();
            builder.AddComicServices();
            return builder.Build();
        }
        public static Task<ComicEntity> GetComicAsync(string address)
        {
            return Default.GetComicAsync(address);
        }
        public static Task DownloadAsync(string address,IComicSaver saver,CancellationToken token=default)
        {
            return Default.DownloadAsync(address,saver,token);
        }
        public static void Download(string address, IComicSaver saver, CancellationToken token = default)
        {
            Default.DownloadAsync(address, saver, token).GetAwaiter().GetResult();
        }
        public static void BatchDownload(string address, IComicSaver saver, int concurrent = 5, CancellationToken token = default)
        {
            Default.BatchDownloadAsync(address, saver,concurrent, token).GetAwaiter().GetResult();
        }
        public static Task BatchDownloadAsync(string address, IComicSaver saver,int concurrent=5, CancellationToken token = default)
        {
            return Default.BatchDownloadAsync(address, saver, concurrent, token);
        }
        public static Task<ComicDetail> GetComicWithChaptersAsync(string address)
        {
            return Default.GetComicWithChaptersAsync(address);
        }
    }
}
