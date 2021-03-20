using Microsoft.Extensions.DependencyInjection;
using System;

namespace Kw.Comic.Engine.Easy.Downloading
{
    public static class DownloadCenterHelper
    {
        public static IDownloadCenter CreateDownloadCenter(this IServiceProvider serviceProvider,
            IDownloadManager downloadManager,
            IComicSaver saver)
        {
            return new DownloadCenter(serviceProvider, downloadManager, saver);
        }
        public static IDownloadCenter CreateQueneDownloadCenter(this IServiceProvider serviceProvider,
            IComicSaver saver)
        {
            return new DownloadCenter(serviceProvider, new QueneDownloadManager(), saver);
        }
        public static IDownloadCenter CreateDownloadCenterFromService(this IServiceProvider serviceProvider)
        {
            var saver = serviceProvider.GetRequiredService<IComicSaver>();
            var mgr = serviceProvider.GetRequiredService<IDownloadManager>();
            return new DownloadCenter(serviceProvider, mgr, saver);
        }
    }
}
