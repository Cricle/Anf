using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class ComicPosTest
    {
        [TestMethod]
        public void GivenValueInit_FieldValueMustEqualToGiven()
        {
            var pos = new ComicPos(1, 2);
            Assert.AreEqual(1, pos.ChapterIndex);
            Assert.AreEqual(2, pos.PageIndex);
            _ = pos.ToString();
        }
        [TestMethod]
        public void GivenLessZeroValueInit_MustThrowExcetpion()
        {
            Assert.ThrowsException<ArgumentException>(() => new ComicPos(-1, 2));
            Assert.ThrowsException<ArgumentException>(() => new ComicPos(1, -2));
            Assert.ThrowsException<ArgumentException>(() => new ComicPos(-1, -2));
        }
        [TestMethod]
        public void GivenSameValueInit_TwoInstanceMustEqual()
        {
            var pos = new ComicPos(1, 2);
            var hash1=pos.GetHashCode();
            var pos2=new ComicPos(1, 2);
            var hash2 = pos2.GetHashCode();
            Assert.AreEqual(pos, pos2);
            Assert.AreEqual(hash1, hash2);
            Assert.IsTrue(pos.Equals(pos2));
            var val=pos.Equals(new object());
            Assert.IsFalse(val);
        }
    }
}
