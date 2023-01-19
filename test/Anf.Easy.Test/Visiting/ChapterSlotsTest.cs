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
    public class ChapterSlotsTest
    {
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ChapterSlots<object>(null));
        }
        [TestMethod]
        public void GivenValueInit_PropertyMustEqualInput()
        {
            var c = new NullComicVisiting<object>();
            var slot = new ChapterSlots<object>(c);
            Assert.AreEqual(c, slot.Visiting);
        }
    }
}
