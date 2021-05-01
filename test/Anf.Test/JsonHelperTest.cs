using Anf.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test
{
    [TestClass]
    public class JsonHelperTest
    {
        class SimpleObject
        {
            public int A { get; set; }

            public string B { get; set; }

            public double C { get; set; }
        }
        [TestMethod]
        public void GivenStr_ConvetToString_AndRev_MustOk()
        {
            var obj = new SimpleObject();
            var str = JsonHelper.Serialize(obj);
            var q = JsonHelper.Deserialize<SimpleObject>(str);
            Assert.IsNotNull(q);
        }
    }
}
