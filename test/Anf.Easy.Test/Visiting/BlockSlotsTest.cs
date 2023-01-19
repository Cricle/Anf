using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class DispoableObject : IDisposable
    {
        public bool IsDisposed { get; set; }
        public void Dispose()
        {
            IsDisposed = true;
        }
    }
    [TestClass]
    public class BlockSlotsTest
    {
        [TestMethod]
        public void GivenMinThanZeroValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => new NullBlockSlots(-1));
            Assert.ThrowsException<ArgumentException>(() => new NullBlockSlots(-10));
            Assert.ThrowsException<ArgumentException>(() => new NullBlockSlots(int.MinValue));
        }
        [TestMethod]
        public async Task GivenDisposeableObject_Dispose_AllDisposed()
        {
            var slot = new NullBlockSlots(10);
            for (int i = 0; i < slot.Size; i++)
            {
                slot.Datas[i] = () => new DispoableObject();
            }
            for (int i = 0; i < slot.Size; i++)
            {
                await slot.GetAsync(i);
            }
            slot.Dispose();
            var objs = slot.GetCreatedValues().OfType<DispoableObject>().ToArray();
            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                Assert.IsTrue(obj.IsDisposed);
            }
        }
        [TestMethod]
        public async Task GivenZeroSize_GetOne_MustGotNothing_ThisGetWasThrowException()
        {
            var slot = new NullBlockSlots(0);
            var res = await slot.GetAsync(0);
            Assert.AreEqual(0, slot.Size);
            Assert.IsNull(res);
            Assert.ThrowsException<IndexOutOfRangeException>(() => slot[1]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => slot[0]);
        }
        [TestMethod]
        public async Task GivenSize_GetTwice_ReturnMustEqual()
        {
            var slot = new NullBlockSlots(1);
            Assert.AreEqual(1, slot.Size);
            slot.Datas[0] = () => new object();
            var a = await slot.GetAsync(0);
            var b = await slot.GetAsync(0);
            Assert.AreEqual(a, b);
            var c = slot[0];
            Assert.AreEqual(c, a);
        }
        [TestMethod]
        public async Task GivenSize_ThisVisit_IsNull_AfterGet_IsNotNull()
        {
            var slot = new NullBlockSlots(1);
            slot.Datas[0] = () => new object();
            var val = slot[0];
            Assert.IsNull(val);
            await slot.GetAsync(0);
            val = slot[0];
            Assert.IsNotNull(val);
        }
        [TestMethod]
        public async Task GivenSize_Get_PageLoadMustBeFired()
        {
            var slot = new NullBlockSlots(1);
            slot.Datas[0] = () => new object();
            BlockSlots<object> sender = null;
            int index = -1;
            object data = null;
            slot.PageLoaded += (o, i, e) =>
            {
                sender = o;
                index = i;
                data = e;
            };
            var inst = await slot.GetAsync(0);
            Assert.AreEqual(slot, sender);
            Assert.AreEqual(0, index);
            Assert.AreEqual(inst, data);

            sender = null;
            index = -1;
            data = null;
            inst = await slot.GetAsync(0);
            Assert.IsNull(sender);
            Assert.AreEqual(-1, index);
            Assert.IsNull(data);

        }
        [TestMethod]
        public async Task GivenSize_MulitAsyncResultWasEqual()
        {
            var slot = new NullBlockSlots(1);
            slot.Datas[0] = () => new object();
            var res = new object[10];
            var tasks = Enumerable.Range(0, res.Length)
                .Select(x => slot.GetAsync(0).ContinueWith(y => res[x] = y.Result))
                .ToArray();
            await Task.WhenAll(tasks);
            var val = slot[0];
            Assert.IsNotNull(val);
            for (int i = 0; i < res.Length; i++)
            {
                if (val!=res[i])
                {
                    Assert.Fail("{0} not equal {1} != {2}", i.ToString(),val,res[i]);
                }
            }
            slot.Dispose();
        }
    }
}
