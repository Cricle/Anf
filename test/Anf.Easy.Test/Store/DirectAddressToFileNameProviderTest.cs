using Anf.Easy.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Easy.Test.Store
{
    [TestClass]
    public class DirectAddressToFileNameProviderTest
    {
        [TestMethod]
        public void GotTwiceInstanceValue_MustBeEqual()
        {
            var a = DirectAddressToFileNameProvider.Instance;
            var b = DirectAddressToFileNameProvider.Instance;
            Assert.AreEqual(a, b);
        }
        [TestMethod]
        public void ConverName_MustCanBeCreateFile()
        {
            var name = "http://localhost:1234/hello@!?a=123&b=(!!!!|)";
            var convertedName = new DirectAddressToFileNameProvider()
                .Convert(name);
            File.Create(convertedName).Dispose();
        }
    }
}
