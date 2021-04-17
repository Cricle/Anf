using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test
{
    [TestClass]
    public class EngineDescriptTest
    {
        [TestMethod]
        public void AddSomeDescript_Get_MustEqualWithAdded()
        {
            var desc = new EngineDescript();
            desc["A"] = "a";
            desc["B"] = "b";
            Assert.AreEqual("a", desc["A"]);
            Assert.AreEqual("b", desc["B"]);
            Assert.AreEqual(2, desc.Count);
        }
        [TestMethod]
        public void GetOrDefault_NothingIsNull()
        {
            var desc = new EngineDescript();
            desc["C"] = "c";
            var val = desc.GetOrDefault("C");
            Assert.AreEqual("c", val);
            Assert.AreEqual(1, desc.Count);
            val = desc.GetOrDefault("--no--");
            Assert.IsNull(val);
        }
    }
}
