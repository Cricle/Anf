using Anf.Easy.Test.Provider;
using Anf.Easy.Test.Visiting;
using Microsoft.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Downloading
{
    [TestClass]
    public class ComicDownloaderExtensionsTest
    {
        [TestMethod]
        public async Task GivenNullOrOutOfRangeValue_MustThrowException()
        {
            var req= new ComicDownloadRequest(new NullSaver(), null, null, new DownloadItemRequest[0], new NullSourceProvider());
            var downloader = new ComicDownloader(new RecyclableMemoryStreamManager());
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => ComicDownloaderExtensions.EmitAsync(null, req));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => ComicDownloaderExtensions.EmitAsync(downloader, null));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => ComicDownloaderExtensions.BatchEmitAsync(null, req));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => ComicDownloaderExtensions.BatchEmitAsync(downloader, null));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => ComicDownloaderExtensions.BatchEmitAsync(downloader, req, 0));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => ComicDownloaderExtensions.BatchEmitAsync(downloader, req, -1));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => ComicDownloaderExtensions.BatchEmitAsync(downloader, req, -5));
        }
        [TestMethod]
        public async Task CallEmit_AllTaskMustBeRun()
        {
            var provider = new ResourceComicProvider();
            var reqs = new DownloadItemRequest[]
            {
                new DownloadItemRequest(null),
                new DownloadItemRequest(null),
                new DownloadItemRequest(null),
                new DownloadItemRequest(null),
            };
            var req = new ComicDownloadRequest(new NullSaver(), null, null, new DownloadItemRequest[0], provider);
            var downloader = new ComicDownloader(new RecyclableMemoryStreamManager());
            await ComicDownloaderExtensions.EmitAsync(downloader, req);
        }
        [TestMethod]
        public async Task CallBatchEmit_AllTaskMustBeRun()
        {
            var provider = new ResourceComicProvider();
            var reqs = new DownloadItemRequest[]
            {
                new DownloadItemRequest(null),
                new DownloadItemRequest(null),
                new DownloadItemRequest(null),
                new DownloadItemRequest(null),
            };
            var req = new ComicDownloadRequest(new NullSaver(), null, null, new DownloadItemRequest[0], provider);
            var downloader = new ComicDownloader(new RecyclableMemoryStreamManager());
            await ComicDownloaderExtensions.BatchEmitAsync(downloader, req);
        }
    }
}
