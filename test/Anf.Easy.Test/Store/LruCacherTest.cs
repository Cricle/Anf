using Anf.Easy.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test.Store
{
    [TestClass]
    public class LruCacherTest
    {
        [TestMethod]
        public void AddNotOutOfRange_MustAllInCache()
        {
            var lru = new LruCacher<int, string>(2);
            lru.Add(1, "a");
            lru.Add(2, "b");
            var dic = lru.Datas;
            Assert.AreEqual(lru.Get(1), dic[1]);
            Assert.AreEqual(lru.Get(2), dic[2]);
            Assert.IsNotNull(lru.SyncRoot);
            Assert.AreEqual(2, lru.Max);
            var aval = lru.Get(1);
            Assert.AreEqual("a", aval);
            var bval = lru.Get(2);
            Assert.AreEqual("b", bval);
            var nval = lru.Get(-1);
            Assert.IsNull(nval);
            var res = lru.TryGetValue(1, out var v);
            Assert.IsTrue(res);
            Assert.AreEqual("a", v);
            res = lru.TryGetValue(888, out v);
            Assert.IsFalse(res);
            Assert.IsNull(v);
        }
        [TestMethod]
        public void AddExistsValue_MustPutItHead()
        {
            var lru = new LruCacher<int, string>(2);
            lru.Add(1, "a");
            lru.Add(2, "b");

            lru.Add(1, "c");
            lru.Add(3, "q");
            Assert.AreEqual("c", lru.Get(1));
            Assert.IsNull(lru.Get(2));
        }
        [TestMethod]
        public void InitWithMinThanZero_MustThrowExcetpion()
        {
            Assert.ThrowsException<ArgumentException>(() => new LruCacher<int, string>(0));
            Assert.ThrowsException<ArgumentException>(() => new LruCacher<int, string>(-1));
            Assert.ThrowsException<ArgumentException>(() => new LruCacher<int, string>(-10));
        }
        [TestMethod]
        public void AddSomeValue_Clear_MustNothingInCache()
        {
            var lru = new LruCacher<int, string>(2);
            lru.Add(1, "a");
            lru.Add(2, "b");
            lru.Clear();
            Assert.AreEqual(0, lru.Count);
        }
        [TestMethod]
        public void AddSomeValue_RemoveItem_RemovedEventMustBeFired()
        {
            var lru = new LruCacher<int, string>(2);
            lru.Add(1, "a");
            int rk=default;
            string rv=default;
            lru.Removed += (a,b) =>
            {
                rk = a;
                rv = b;
            };
            lru.Remove(1,out _);
            Assert.AreEqual(1, rk);
            Assert.AreEqual("a", rv);
        }
        [TestMethod]
        public void GetOrAdd_ValueMustBeAddOrGet()
        {
            var modify = 0;
            var lru = new LruCacher<int, string>(2);
            var v=lru.GetOrAdd(1, () =>
             {
                 modify++;
                 return "a";
             });
            var val = lru.Get(1);
            Assert.AreEqual("a", v);
            Assert.AreEqual("a", val);
            Assert.AreEqual(1, lru.Count);
            v=lru.GetOrAdd(1, () =>
            {
                modify++;
                return "a";
            });
            Assert.AreEqual(1, lru.Count);
            Assert.AreEqual("a", v);
        }
        [TestMethod]
        public void AddSomeValue_ContainsExisted_MustReturnTrue()
        {
            var lru = new LruCacher<int, string>(2);
            lru.Add(1, "a");
            var val = lru.ContainsKey(1);
            Assert.IsTrue(val);
            val = lru.ContainsKey(3);
            Assert.IsFalse(val);
        }
        [TestMethod]
        public void AddSomeValue_RemoveIt_ItemMustRemoved()
        {
            var lru = new LruCacher<int, string>(2);
            lru.Add(1, "a");
            lru.Add(2, "b");
            var val=lru.Remove(1,out var v);
            Assert.IsTrue(val);
            Assert.AreEqual(v, "a");
            Assert.AreEqual(1, lru.Count);
            val=lru.Remove(99, out v);
            Assert.IsFalse(val);
            Assert.IsNull(v);
        }
        [TestMethod]
        public void AddOutOfRange_MustRemovedPrevItem()
        {
            var lru = new LruCacher<int, string>(2);
            Assert.AreEqual(2, lru.Max);
            lru.Add(1, "a");
            lru.Add(2, "b");
            lru.Add(3, "c");

            var val1 = lru.Get(1);
            Assert.IsNull(val1);
            Assert.AreEqual(2, lru.Count);
            lru.Get(3);
            lru.Get(2);
            lru.Add(4, "d");
            var val2 = lru.Get(3);
            Assert.IsNull(val2);
            var val3 = lru.Get(2);
            Assert.AreEqual("b", val3);
        }
    }
}
