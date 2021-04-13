using Anf.Easy.Test.Provider;
using Microsoft.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Downloading
{
    [TestClass]
    public class ComicDownloaderTest
    {
        [TestMethod]
        public void GivenNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ComicDownloader(null));
        }
        [TestMethod]
        public void GivenAnyNullValueToEmit_MustThrowException()
        {
            var mgr = new RecyclableMemoryStreamManager();
            var sourceProvider = new ResourceComicProvider();
            var saver = new NullSaver();
            Assert.ThrowsException<ArgumentNullException>(() => new ComicDownloader(mgr).EmitTasks(null));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicDownloader(mgr).EmitTasks(new ComicDownloadRequest(saver, null, null, null, sourceProvider)));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicDownloader(mgr).EmitTasks(new ComicDownloadRequest(null, null, null, new DownloadItemRequest[0], sourceProvider)));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicDownloader(mgr).EmitTasks(new ComicDownloadRequest(saver, null, null, null, sourceProvider)));
        }
        private ComicDownloadRequest CreateRequest(int count)
        {
            var sourceProvider = new ResourceComicProvider();
            var saver = new NullSaver();
            var reqs = Enumerable.Range(0, count)
                .Select(x => new DownloadItemRequest(new ComicPage { TargetUrl = "-no-" }))
                .ToArray();
            var req = new ComicDownloadRequest(saver, null, null, reqs, sourceProvider);
            return req;
        }
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(10)]
        public void GivenAnyRequest_Emit_MustReturnAllTasks(int count)
        {
            var mgr = new RecyclableMemoryStreamManager();
            var req = CreateRequest(count);
            var downloader = new ComicDownloader(mgr);
            var tasks = downloader.EmitTasks(req);
            Assert.AreEqual(count, tasks.Length);
        }
        [TestMethod]
        public void GivenAnyRequest_CancelIt_MustNothingReturn()
        {
            var mgr = new RecyclableMemoryStreamManager();
            var req = CreateRequest(10);
            var tks = new CancellationTokenSource();
            tks.Cancel();
            var downloader = new ComicDownloader(mgr);
            var res = downloader.EmitTasks(req, tks.Token);
            Assert.AreEqual(0, res.Length);
        }
        [TestMethod]
        public async Task EmitTask_MustReturnValue()
        {
            var mgr = new RecyclableMemoryStreamManager();
            var req = CreateRequest(1);
            var downloader = new ComicDownloader(mgr);
            var res = downloader.EmitTasks(req);
            await res[0]();
        }
        [TestMethod]
        public async Task EmitTaskWithListener_ListenerMethodWasCalled()
        {
            var mgr = new RecyclableMemoryStreamManager();
            var req = CreateRequest(1);
            var listener = new NullDownloadListener();
            req.Listener = listener;
            var downloader = new ComicDownloader(mgr);
            var res = downloader.EmitTasks(req);
            await res[0]();
            Assert.IsTrue(listener.IsReadyFetchAsync);
            Assert.IsTrue(listener.IsBeginFetchPageAsync);
            Assert.IsTrue(listener.IsReadySaveAsync);
            Assert.IsTrue(listener.IsEndFetchPageAsync);
            Assert.IsTrue(listener.IsComplatedSaveAsync);
        }
        [TestMethod]
        public async Task EmitTaskWithListener_Cancel_CancelListenerWasCalled()
        {
            var mgr = new RecyclableMemoryStreamManager();
            var req = CreateRequest(1);
            var listener = new NullDownloadListener();
            req.Listener = listener;
            var downloader = new ComicDownloader(mgr);
            var tks = new CancellationTokenSource();
            var res = downloader.EmitTasks(req,tks.Token);
            tks.Cancel();
            try
            {
                await res[0]();
            }
            catch (Exception) { }
            Assert.IsTrue(listener.IsCanceledAsync);
        }
        [TestMethod]
        public async Task EmitTaskWithListener_NotNeedToSave_TheMethodWasCalled()
        {
            var mgr = new RecyclableMemoryStreamManager();
            var req = CreateRequest(1);
            var listener = new NullDownloadListener();
            req.Listener = listener;
            var saver = (NullSaver)req.Saver;
            saver.IsNeedToSave = false;
            var downloader = new ComicDownloader(mgr);
            var res = downloader.EmitTasks(req);
            await res[0]();
            Assert.IsTrue(listener.IsNotNeedToSaveAsync);
        }
        [TestMethod]
        public async Task EmitTaskWithListener_ThrowException_TheMethodWasCalled()
        {
            var sourceProvider = new ExceptionComicProvider();
            var saver = new NullSaver();
            var reqs = Enumerable.Range(0, 1)
                .Select(x => new DownloadItemRequest(new ComicPage { TargetUrl = "-no-" }))
                .ToArray();
            var mgr = new RecyclableMemoryStreamManager();
            var req = new ComicDownloadRequest(saver, null, null, reqs, sourceProvider);
            var downloader = new ComicDownloader(mgr);
            var listener = new NullDownloadListener();
            req.Listener = listener;
            var res = downloader.EmitTasks(req);
            await res[0]();
            Assert.IsTrue(listener.IsFetchPageExceptionAsync);
        }
    }
}
