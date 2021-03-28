using Kw.Comic.Engine.Easy;
using Kw.Comic.Engine.Easy.Downloading;
using Kw.Comic.Engine.Easy.Store;
using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace Kw.Comic
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCoreServices(this IServiceCollection services,string storePath)
        {
            services.AddSingleton<IResourceFactoryCreator<Stream>>(StreamResourceFactoryCreator.Default);
            var fs = FileStoreService.FromMd5Default(storePath);
            services.AddSingleton<IStoreService>(fs);
            services.AddSingleton<IComicSaver>(fs);
            services.AddSingleton(fs);
            services.AddSingleton<IDownloadCenter>(x =>
            {
                var store = x.GetRequiredService<IComicSaver>();
                var center = new DownloadCenter(x, new QueneDownloadManager(), store);
                center.Start();
                return center;
            });
        }
    }
}
