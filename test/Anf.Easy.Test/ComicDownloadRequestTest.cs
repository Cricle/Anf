using Anf.Easy.Test.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class ComicDownloadRequestTest
    {
        [TestMethod]
        public void GivenValueInit_PropertyMustEqualGiven()
        {
            var saver = new NullSaver();
            var entity = new ComicEntity();
            var detail = new ComicDetail();
            var reqs = new List<DownloadItemRequest>();
            var provider = new NullSourceProvider();
            var req = new ComicDownloadRequest(saver, entity, detail, reqs, provider);
            Assert.AreEqual(saver, req.Saver);
            Assert.AreEqual(entity, req.Entity);
            Assert.AreEqual(detail, req.Detail);
            Assert.AreEqual(reqs, req.DownloadRequests);
            Assert.AreEqual(provider, req.Provider);
            var listener = new NullDownloadListener();
            req.Listener = listener;
            Assert.AreEqual(listener, req.Listener);
        }
    }
}
