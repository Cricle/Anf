using Anf.Easy.Concurrnets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test.Concurrnets
{
    [TestClass]
    public class KwSynchronizedCollectionTest
    {
        [TestMethod]
        public void InitWithSyncRoot_MustEquipInput()
        {
            var obj = new object();
            var coll = new KwSynchronizedCollection<int>();
            Assert.IsNotNull(coll);
            coll = new KwSynchronizedCollection<int>(obj);
            Assert.AreEqual(obj,coll.SyncRoot);
        }
        [TestMethod]
        public void InitWithData_MustHasDatas()
        {
            var coll = new KwSynchronizedCollection<int>(new object(),1,2,3,4,5);
            Assert.AreEqual(5, coll.Count);
        }
        [TestMethod]
        public void OperaotrCollection_MustOk()
        {
            var coll = new KwSynchronizedCollection<int>();
            Assert.AreEqual(0, coll.Count);
            coll.Add(1);
            Assert.AreEqual(1, coll.Count);
            Assert.AreEqual(1, coll[0]);
            coll.Remove(1);
            Assert.AreEqual(0, coll.Count);
            coll.Insert(0, 1);
            Assert.AreEqual(1, coll.Count);
            coll.Add(0);
            coll.Add(1);
            coll.Add(2);
            coll.Clear();
            Assert.AreEqual(0, coll.Count);
            coll.Add(1);
            var idx = coll.IndexOf(1);
            Assert.AreEqual(0, idx);
            idx = coll.IndexOf(3);
            coll.Clear();
            Assert.AreEqual(-1, idx);
            coll.Add(1);
            coll.RemoveAt(0);
            Assert.AreEqual(0, coll.Count);
        }
        [TestMethod]
        public void AddOutOfRange_MustThrowExeption()
        {
            var coll = new KwSynchronizedCollection<int>();
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => coll[1] = 1);

        }
    }
}
