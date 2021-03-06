using Kw.Comic.Engine.Networks;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    internal class ComicDownloader : IComicDownloader
    {
        private readonly RecyclableMemoryStreamManager streamManager;

        public ComicDownloader(RecyclableMemoryStreamManager streamManager)
        {
            this.streamManager = streamManager;
        }

        public Func<Task>[] EmitTasks(ComicDownloadRequest request, CancellationToken token = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (request.Provider is null || request.Saver is null || request.Detail is null)
            {
                throw new ArgumentNullException("Any for " + nameof(request));
            }
            var tasks = new List<Func<Task>>();
            foreach (var item in request.Detail.Chapters)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                foreach (var page in item.Pages)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                    tasks.Add(() => DownloadPageAsync(request, item, page, token));
                }
            }
            return tasks.ToArray();
        }
        private async Task DownloadPageAsync(ComicDownloadRequest request, ChapterWithPage chapter, ComicPage page, CancellationToken token)
        {
            using (var stream = await request.Provider.GetImageStreamAsync(page.TargetUrl))
            using (var destStream = streamManager.GetStream())
            {
                await stream.CopyToAsync(destStream);
                destStream.Seek(0, SeekOrigin.Begin);
                var ctx = new ComicDownloadContext(request.Detail, chapter, page, destStream, token);
                await request.Saver.SaveAsync(ctx);
            }
        }
    }
}
