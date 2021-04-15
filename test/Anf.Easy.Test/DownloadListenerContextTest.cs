using Anf.Easy.Test.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class DownloadListenerContextTest
    {
        [TestMethod]
        public void GivenValue_PropertyValueMustEqualGiven()
        {
            var saver = new NullComicSaver();
            var req = new DownloadItemRequest[0];
            var prov = new NullSourceProvider();
            var eq = new ComicDownloadRequest(saver, null, null, req,prov);
            var chp = new ComicChapter();
            var page = new ComicPage();
            var ctx = new DownloadListenerContext(eq, chp, page, default);
            Assert.AreEqual(eq, ctx.Request);
            Assert.AreEqual(chp, ctx.Chapter);
            Assert.AreEqual(page, ctx.Page);
            Assert.AreEqual(CancellationToken.None, ctx.Token);
        }
    }
}
