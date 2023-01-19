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
    public class DataCursorBaseTest
    {
        [TestMethod]
        public async Task Move_WhenSuccedPropertyChanged_FailNothingTodo()
        {
            var rand = new Random();
            var range = Enumerable.Range(0, 10)
                .Select(x => (object)rand.Next(0, 999))
                .ToArray();
            var cur = new NullDataCursor(range);
            Assert.AreEqual(range.Length, cur.Count);
            Assert.AreEqual(-1, cur.CurrentIndex);

            var ok = await cur.MoveAsync(0);
            Assert.IsTrue(ok);
            Assert.AreEqual(0, cur.CurrentIndex);
            Assert.AreEqual(range[0], cur.Current);

            ok = await cur.MoveAsync(1);
            Assert.IsTrue(ok);
            Assert.AreEqual(1, cur.CurrentIndex);
            Assert.AreEqual(range[1], cur.Current);

            ok = await cur.MoveAsync(-1);
            Assert.IsFalse(ok);
            Assert.AreEqual(1, cur.CurrentIndex);
            Assert.AreEqual(range[1], cur.Current);

            ok = await cur.MoveAsync(9999999);
            Assert.IsFalse(ok);
            Assert.AreEqual(1, cur.CurrentIndex);
            Assert.AreEqual(range[1], cur.Current);
            cur.Dispose();
        }
        [TestMethod]
        public async Task Move_MovedEventMustBeFired()
        {
            var cur = new NullDataCursor(new object[] { new object() });
            IDataCursor<object> sender = null;
            int index = -1;
            cur.Moved += (o, e) =>
            {
                sender = o;
                index = e;
            };
            await cur.MoveAsync(0);
            Assert.AreEqual(cur, sender);
            Assert.AreEqual(0, index);

            sender = null;
            index = -1;
            await cur.MoveAsync(0);
        }
        [TestMethod]
        public async Task Move_CallbackWasCalled()
        {
            var cur = new NullDataCursor(new object[] { new object() });
            await cur.MoveAsync(0);
            Assert.IsTrue(cur.IsOnMoved);
            await cur.MoveAsync(-1);
            Assert.IsTrue(cur.IsOnSkiped);
        }
        [TestMethod]
        public void Move_GetSetEnableSafeSet_ValueMustEqualSet()
        {
            var cur = new NullDataCursor(null);
            cur.EnableSafeSet = true;
            Assert.IsTrue(cur.EnableSafeSet);
            cur.EnableSafeSet = false;
            Assert.IsFalse(cur.EnableSafeSet);
        }
#if NET461_OR_GREATER||NETCOREAPP3_1

        public async Task GetAsyncEnumerable_MoveAll_AllGot()
        {
            var datas = Enumerable.Range(0, 100).Select(x => new object()).ToArray();
            var cur = new NullDataCursor(datas);
            var c = cur.GetAsyncEnumerator();
            var index = 0;
            while (await c.MoveNextAsync())
            {
                var val = c.Current;
                Assert.AreEqual(datas[index], val);
            }
        }
#endif
    }
}
