using Microsoft.Extensions.DependencyInjection;
using System;

namespace Anf.Easy.Downloading
{
    public static class DownloadCenterHelper
    {
        public static IDownloadCenter CreateDownloadCenter(this IServiceProvider serviceProvider,
            IDownloadManager downloadManager,
            IComicSaver saver)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return new DownloadCenter(serviceProvider, downloadManager, saver);
        }
        public static IDownloadCenter CreateQueneDownloadCenter(this IServiceProvider serviceProvider,
            IComicSaver saver)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            return new DownloadCenter(serviceProvider, new QueneDownloadManager(), saver);
        }
        public static IDownloadCenter CreateDownloadCenterFromService(this IServiceProvider serviceProvider)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var saver = serviceProvider.GetRequiredService<IComicSaver>();
            var mgr = serviceProvider.GetRequiredService<IDownloadManager>();
            return new DownloadCenter(serviceProvider, mgr, saver);
        }
    }
}
