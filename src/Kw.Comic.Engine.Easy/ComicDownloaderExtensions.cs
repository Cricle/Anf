using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    public static class ComicDownloaderExtensions
    {
        public static async Task EmitAsync(this IComicDownloader downloader,
            ComicDownloadRequest request,
            CancellationToken token = default)
        {
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
            var tasks = downloader.EmitTasks(request, token);
            return TaskQuene.RunVoidAsync(tasks, concurrent);
        }
    }
}
