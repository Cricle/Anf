using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class ComicChapterManagerExtensionsTest
    {
        [TestMethod]
        public void GivenNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ComicChapterManagerExtensions.CreatePageSlots<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => ComicChapterManagerExtensions.CreateChapterSlots<object>(null));
        }
        [TestMethod]
        public void GivenChapterManager_MustCreatedSlots()
        {
            var mgr = new ValueComicChapterManager<object>();
            var slot = ComicChapterManagerExtensions.CreatePageSlots(mgr);
            Assert.IsNotNull(slot);
            Assert.AreEqual(mgr, slot.ChapterManager);
        }
        [TestMethod]
        public void GivenPageManager_MustCreatedSlots()
        {
            var mgr = new NullComicVisiting<object>();
            var slot = ComicChapterManagerExtensions.CreateChapterSlots(mgr);
            Assert.IsNotNull(slot);
            Assert.AreEqual(mgr, slot.Visiting);
        }
    }
}
