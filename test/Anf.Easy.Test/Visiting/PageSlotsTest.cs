using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class PageSlotsTest
    {
        [TestMethod]
        public void GivenNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PageSlots<object>(null));
        }
        [TestMethod]
        public void GivenValueInit_PropertyValueMustEqualGiven()
        {
            var mgr = new NullComicChapterManager<object>();
            var slot = new PageSlots<object>(mgr);
            Assert.AreEqual(mgr, slot.ChapterManager);
        }
        [TestMethod]
        public async Task GetOne_MustInitWithChapterManager()
        {
            var instance0 = new NullComicVisitPage<object>();
            var instance1 = new NullComicVisitPage<object>();
            var instance2 = new NullComicVisitPage<object>();

            var mgr = new ValueComicChapterManager<object> 
            {
                Map=new Dictionary<int, Func<IComicVisitPage<object>>>
                {
                    [0] = () => instance0,
                    [1] = () => instance1,
                    [2] = () => instance2,
                }
            };
            mgr.ChapterWithPage = new ChapterWithPage(null, Enumerable.Range(0, mgr.Map.Count).Select(x => new ComicPage()).ToArray());
            var slot = new PageSlots<object>(mgr);
            var value0 = await slot.GetAsync(0);
            var value1 = await slot.GetAsync(1);
            var value2 = await slot.GetAsync(2);

            Assert.AreEqual(instance0, value0);
            Assert.AreEqual(instance1, value1);
            Assert.AreEqual(instance2, value2);
        }
    }
}
