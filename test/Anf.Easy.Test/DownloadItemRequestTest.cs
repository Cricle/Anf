using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test
{
    [TestClass]
    public class DownloadItemRequestTest
    {
        [TestMethod]
        public void GivenValueInit_PropertyValueMustEqualGiven()
        {
            var chapter = new ComicChapter();
            var page = new ComicPage();
            var req = new DownloadItemRequest(chapter,page);
            Assert.AreEqual(chapter, req.Chapter);
            Assert.AreEqual(page, req.Page);
            req = new DownloadItemRequest(page);
            Assert.IsNull(req.Chapter);
            Assert.AreEqual(page, req.Page);
        }
    }
}
