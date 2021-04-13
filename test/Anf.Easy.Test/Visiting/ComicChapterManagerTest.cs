using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class ComicChapterManagerTest
    {
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ComicChapterManager<Stream>(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicChapterManager<Stream>(new ChapterWithPage(null,null), null));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicChapterManager<Stream>(null, ComicVisitingHelper.CreateResrouceVisitor()));
        }
        [TestMethod]
        public void GivenValueInit_PropertyValueMustEqualGiven()
        {
            var page = new ChapterWithPage(null, null);
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            var mgr = new ComicChapterManager<Stream>(page, visit);
            Assert.AreEqual(page, mgr.ChapterWithPage);
            Assert.AreEqual(visit, mgr.ComicVisiting);
        }
        [TestMethod]
        public async Task GivenValueInit_ButNullResourceFactory_GetVisitPage_MustThrowException()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => new ComicChapterManager<Stream>(new ChapterWithPage(null,null), visit).GetVisitPageAsync(0));
        }
        [TestMethod]
        public async Task GivenValueInit_GetVisitPage_MustCallIntercepts()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            var intercept = new NullComicVisitingInterceptor<Stream>();
            visit.VisitingInterceptor = intercept;
            await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            await visit.LoadChapterAsync(0);
            var mgr = new ComicChapterManager<Stream>(visit.ChapterWithPages[0], visit);
            var pg = await mgr.GetVisitPageAsync(0);
            Assert.IsNotNull(pg);
            Assert.IsNotNull(pg.Page);
            Assert.IsNotNull(pg.Resource);


            Assert.IsTrue(intercept.IsGettingPageAsync);
            Assert.IsTrue(intercept.IsGotPageAsync);
        }
    }
}
