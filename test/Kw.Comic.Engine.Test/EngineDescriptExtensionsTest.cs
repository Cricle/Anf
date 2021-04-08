using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test
{
    [TestClass]
    public class EngineDescriptExtensionsTest
    {
        [TestMethod]
        public void SetNormalDescript_Get_MustEqualSet()
        {
            var desc = "desc";
            var name = "name";
            var url = "url";
            var d = new EngineDescript();
            d.SetDescript(desc);
            d.SetName(name);
            d.SetUrl(url);
            Assert.AreEqual(desc, d[EngineDescriptConst.Descript]);
            Assert.AreEqual(name, d[EngineDescriptConst.Name]);
            Assert.AreEqual(url, d[EngineDescriptConst.Url]);
        }
    }
}
