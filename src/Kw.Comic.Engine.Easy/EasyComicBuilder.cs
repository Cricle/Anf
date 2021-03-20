using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
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
        public static Task<ComicEntity> GetComicAsync(string address)
        {
            return Default.GetComicAsync(address);
        }
        public static Task DownloadAsync(string address, IComicSaver saver, CancellationToken token = default)
        {
            return Default.DownloadAsync(address, saver, token);
        }
        public static void Download(string address, IComicSaver saver, CancellationToken token = default)
        {
            Default.DownloadAsync(address, saver, token).GetAwaiter().GetResult();
        }
        public static void BatchDownload(string address, IComicSaver saver, int concurrent = 5, CancellationToken token = default)
        {
            Default.BatchDownloadAsync(address, saver, concurrent, token).GetAwaiter().GetResult();
        }
        public static Task BatchDownloadAsync(string address, IComicSaver saver, int concurrent = 5, CancellationToken token = default)
        {
            return Default.BatchDownloadAsync(address, saver, concurrent, token);
        }
        public static Task<ComicDetail> GetComicWithChaptersAsync(string address)
        {
            return Default.GetComicWithChaptersAsync(address);
        }
    }
}
