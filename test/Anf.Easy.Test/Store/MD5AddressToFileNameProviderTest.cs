using Anf.Easy.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test.Store
{
    [TestClass]
    public class MD5AddressToFileNameProviderTest
    {
        [TestMethod]
        public void Init_ConvertTwice_ReturnMustEqual()
        {
            var addr = "fjdsgvfigiug4rf9e8gf./.@#@";
            var prov = new MD5AddressToFileNameProvider();
            var prov2 = MD5AddressToFileNameProvider.Instance;
            var a = prov.Convert(addr);
            var b = prov2.Convert(addr);
            Assert.AreEqual(a, b);
            prov.Dispose();
        }
    }
}
