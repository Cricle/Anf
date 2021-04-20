using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class ObjectUsingPoolTest
    {
        [TestMethod]
        public async Task GetOne_AfterGet_ValueMustEqual()
        {
            var pool = new ValueObjectUsingPool();
            var a =await pool.GetAsync(1);
            var b =await pool.GetAsync(1);
            Assert.AreEqual(a, b);
            Assert.AreEqual(1,pool.Keys.Single());
            Assert.IsTrue(pool.ContainsKey(1));
            Assert.IsFalse(pool.ContainsKey(0));
            var val = pool.GetUseCount(1);
            Assert.AreEqual(2, val.Value);
            var count = pool.GetUseCount(0);
            Assert.IsNull(count);
        }
        [TestMethod]
        public async Task GetOne_ReturnOne_ValueMustBeDetched()
        {
            var pool = new ValueObjectUsingPool();
            var a = await pool.GetAsync(1);
            pool.Return(1);
            Assert.AreEqual(0, pool.Count);
        }
        [TestMethod]
        public async Task GetOne_ReturnInRunning_AfterMustBeDisposed()
        {
            var pool = new DisposableObjectUsingPool();
            pool.WaitTime = TimeSpan.FromSeconds(4);
            var a = pool.GetAsync(1);
            pool.Return(1);
            var obj=await a;
            await Task.Delay(1000);
            Assert.IsTrue(obj.IsDisposed);
            Assert.AreEqual(0, pool.Count);
        }
        [TestMethod]
        public async Task GetAny_DisposeIt_ItMustBeDisposed()
        {
            var pool = new DisposableObjectUsingPool();
            var dispObjs = new DispoableObject[10];
            for (int i = 0; i < dispObjs.Length; i++)
            {
                dispObjs[i]= await pool.GetAsync(i);
            }
            for (int i = 0; i < dispObjs.Length; i++)
            {
                pool.Return(i);
                Assert.IsTrue(dispObjs[i].IsDisposed, i.ToString());
            }
        }
        [TestMethod]
        public async Task GetAny_DisposeAll_AllMustBeDisposed()
        {
            var pool = new DisposableObjectUsingPool();
            var dispObjs = new DispoableObject[10];
            for (int i = 0; i < dispObjs.Length; i++)
            {
                dispObjs[i] = await pool.GetAsync(i);
            }
            pool.Dispose();
            for (int i = 0; i < dispObjs.Length; i++)
            {
                Assert.IsTrue(dispObjs[i].IsDisposed, i.ToString());
            }
        }
        [TestMethod]
        public async Task ConcurrentGet_ValueMustEqual()
        {
            var pool = new ValueObjectUsingPool();
            var objs = new object[20];
            var tasks = Enumerable.Range(0, objs.Length)
                .Select(x => pool.GetAsync(1).ContinueWith(y => objs[x] = y.Result))
                .ToArray();
            await Task.WhenAll(tasks);
            var val =await pool.GetAsync(1);
            Assert.IsNotNull(val);
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i]!=val)
                {
                    Assert.Fail(i.ToString());
                }
            }
            var count = pool.GetUseCount(1);
            Assert.AreEqual(21, count.Value);
        }
    }
}
