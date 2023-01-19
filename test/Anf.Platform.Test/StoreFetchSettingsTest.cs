using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Test
{
    [TestClass]
    public class StoreFetchSettingsTest
    {
        [TestMethod]
        public void NoCache_MustTagNoCahce()
        {
            var s = StoreFetchSettings.NoCache;
            Assert.IsTrue(s.ForceNoCache);
        }
        [TestMethod]
        public void DefaultCache()
        {
            var s = StoreFetchSettings.DefaultCache;
            Assert.IsFalse(s.ForceNoCache);
            Assert.AreEqual(StoreFetchSettings.DefaultExpiresTime, s.ExpiresTime);
            Assert.IsTrue(s.DisposeStream);
        }
        [TestMethod]
        public void InitWithArgs_PropertyMustEqualInput()
        {
            var t = TimeSpan.FromSeconds(10);
            var s = new StoreFetchSettings(true, t);
            Assert.IsTrue(s.ForceNoCache);
            Assert.AreEqual(t, s.ExpiresTime);
        }
        [TestMethod]
        public void Clone_AllPropertyMustEqual()
        {
            var a = new StoreFetchSettings(true, TimeSpan.FromSeconds(10)) { DisposeStream = true };

            var b = a.Clone();
            Assert.AreEqual(a.DisposeStream, b.DisposeStream);
            Assert.AreEqual(a.ExpiresTime, b.ExpiresTime);
            Assert.AreEqual(a.ForceNoCache, b.ForceNoCache);
        }
    }
}
