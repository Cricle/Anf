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
    public class DataCursorExtensionsTest
    {
        private NullDataCursor CreateCursor(int size)
        {
            var data = Enumerable.Range(0, size).Select(x => new object()).ToArray();
            var cur = new NullDataCursor(data);
            return cur;
        }
        [TestMethod]
        public void GivenNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => DataCursorExtensions.IsEnd<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DataCursorExtensions.IsFirst<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DataCursorExtensions.IsInRange<object>(null, 0));
            Assert.ThrowsException<ArgumentNullException>(() => DataCursorExtensions.MoveFirstAsync<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DataCursorExtensions.MoveLastAsync<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DataCursorExtensions.MoveNextAsync<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DataCursorExtensions.MovePrevAsync<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => DataCursorExtensions.RightCount<object>(null));
        }
        [TestMethod]
        [DataRow(10, 0)]
        [DataRow(10, 1)]
        [DataRow(10, 5)]
        [DataRow(10, 9)]
        [DataRow(1, 0)]
        [DataRow(2, 0)]
        [DataRow(2, 1)]
        public void GivenInRangeIndex_MustReturnTrue(int size,int index)
        {
            var cur = CreateCursor(size);
            var val = DataCursorExtensions.IsInRange(cur, index);
            Assert.IsTrue(val);
        }
        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(0, 2)]
        [DataRow(1, 1)]
        [DataRow(5, 10)]
        [DataRow(20, int.MaxValue)]
        [DataRow(20, -1)]
        [DataRow(20, -99)]
        public void GivenNotInRangeIndex_MustReturnFalse(int size, int index)
        {
            var cur = CreateCursor(size);
            var val = DataCursorExtensions.IsInRange(cur, index);
            Assert.IsFalse(val);
        }
        [TestMethod]
        public void ZeroCursor_IsFirstOrEndMustReturnFalse()
        {
            var cur = CreateCursor(0);
            Assert.IsFalse(DataCursorExtensions.IsFirst(cur));
            Assert.IsFalse(DataCursorExtensions.IsEnd(cur));
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(20)]
        [DataRow(40)]
        public async Task GivenEndOrNotCursor_MustReturnIsEnd(int size)
        {
            var cur = CreateCursor(size);
            await cur.MoveAsync(size - 1);
            Assert.IsTrue(DataCursorExtensions.IsEnd(cur));
            for (int i = 0; i < size - 2; i++)
            {
                await cur.MoveAsync(i);
                Assert.IsFalse(DataCursorExtensions.IsEnd(cur));
            }
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(20)]
        [DataRow(40)]
        public async Task GivenFirstOrNotCursor_MustReturnIsEnd(int size)
        {
            var cur = CreateCursor(size);
            await cur.MoveAsync(0);
            Assert.IsTrue(DataCursorExtensions.IsFirst(cur));
            for (int i = 1; i < size-1; i++)
            {
                await cur.MoveAsync(i);
                Assert.IsFalse(DataCursorExtensions.IsFirst(cur));
            }
        }
        [TestMethod]
        [DataRow(0,0)]
        [DataRow(1,0)]
        [DataRow(2, 1)]
        [DataRow(2, 0)]
        [DataRow(5,2)]
        [DataRow(10,9)]
        [DataRow(40,20)]
        public async Task GetRightCount_ReturnMustRightSize(int size,int index)
        {
            var cur = CreateCursor(size);
            await cur.MoveAsync(index);
            var right = Math.Max(0, size - index - 1);
            Assert.AreEqual(right, DataCursorExtensions.RightCount(cur));
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(7)]
        [DataRow(10)]
        public async Task IndexNotFirst_MoveFirst_CursorMustAtFirst(int size)
        {
            var cur = CreateCursor(size);
            for (int i = 1; i < size; i++)
            {
                await cur.MoveAsync(i);
                await cur.MoveFirstAsync();
                Assert.AreEqual(0, cur.CurrentIndex);
            }
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(7)]
        [DataRow(10)]
        public async Task IndexNotLast_MoveLast_CursorMustAtLast(int size)
        {
            var cur = CreateCursor(size);
            for (int i = 0; i < size - 2; i++)
            {
                await cur.MoveAsync(i);
                await cur.MoveLastAsync();
                Assert.AreEqual(size - 1, cur.CurrentIndex);
            }
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(7)]
        [DataRow(10)]
        public async Task IndexWasAny_MoveNext_CursorMustMoveNext(int size)
        {
            var cur = CreateCursor(size);
            for (int i = 0; i < size; i++)
            {
                await cur.MoveNextAsync();
                Assert.AreEqual(i, cur.CurrentIndex);
            }
        }
        [TestMethod]
        [DataRow(5)]
        [DataRow(7)]
        [DataRow(10)]
        public async Task IndexWasAny_MovePrev_CursorMustMovePrev(int size)
        {
            var cur = CreateCursor(size);
            await cur.MoveAsync(size - 1);
            var idx = cur.CurrentIndex;
            for (int i = size - 1; i > 1; i--)
            {
                await cur.MovePrevAsync();
                idx--;
                Assert.AreEqual(idx, cur.CurrentIndex);
            }
        }
    }
}
