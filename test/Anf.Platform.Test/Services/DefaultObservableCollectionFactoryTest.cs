using Anf.Platform.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Test.Services
{
    [TestClass]
    public class DefaultObservableCollectionFactoryTest
    {
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            var fc = new DefaultObservableCollectionFactory();
            var coll = fc.Create<object>();
            var datas = new[] { new object(), new object() };
            Assert.ThrowsException<ArgumentNullException>(() => fc.Create<object>(null));
            Assert.ThrowsException<ArgumentNullException>(() => fc.AddRange(null,datas));
            Assert.ThrowsException<ArgumentNullException>(() => fc.AddRange(coll,null));
        }
        [TestMethod]
        public void CreateCollection_MustCreated()
        {
            var fc = new DefaultObservableCollectionFactory();
            var coll = fc.Create<object>();
            Assert.IsNotNull(coll);
        }
        [TestMethod]
        public void CreateWithDatas_MustContains()
        {
            var fc = new DefaultObservableCollectionFactory();
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            var coll = fc.Create(new[] { o1, o2, o3, o4 });
            Assert.IsTrue(coll.Contains(o1));
            Assert.IsTrue(coll.Contains(o2));
            Assert.IsTrue(coll.Contains(o3));
            Assert.IsTrue(coll.Contains(o3));
            Assert.IsTrue(coll.Contains(o4));
        }
        [TestMethod]
        public void AllRange_MustConains()
        {
            var fc = new DefaultObservableCollectionFactory();
            var coll = fc.Create<object>();
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            fc.AddRange(coll, new[] { o1, o2, o3, o4 });
            Assert.IsTrue(coll.Contains(o1));
            Assert.IsTrue(coll.Contains(o2));
            Assert.IsTrue(coll.Contains(o3));
            Assert.IsTrue(coll.Contains(o3));
            Assert.IsTrue(coll.Contains(o4));
        }
    }
}
