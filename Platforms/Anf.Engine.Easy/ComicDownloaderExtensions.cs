using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy
{
    public static class ComicDownloaderExtensions
    {
        public static async Task EmitAsync(this IComicDownloader downloader,
            ComicDownloadRequest request,
            CancellationToken token = default)
        {
            if (downloader is null)
            {
                throw new ArgumentNullException(nameof(downloader));
            }

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var tasks = downloader.EmitTasks(request, token);
            foreach (var item in tasks)
            {
                await item();
            }
        }
        public static Task BatchEmitAsync(this IComicDownloader downloader,
           ComicDownloadRequest request,
           int concurrent = 5,
           CancellationToken token = default)
        {
            if (downloader is null)
            {
                throw new ArgumentNullException(nameof(downloader));
            }

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (concurrent <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(concurrent));
            }

            var tasks = downloader.EmitTasks(request, token);
            return TaskQuene.RunVoidAsync(tasks, concurrent);
        }
    }
}
