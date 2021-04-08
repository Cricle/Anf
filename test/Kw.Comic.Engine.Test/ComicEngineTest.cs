using Kw.Comic.Engine.Test.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kw.Comic.Engine.Test
{
    [TestClass]
    public class ComicEngineTest
    {
        [TestMethod]
        public void GivenNotUri_MustReturnThrowException()
        {
            var eng = new ComicEngine();
            Assert.ThrowsException<UriFormatException>(()=> eng.GetComicSourceProviderType("--not-uri--"));
        }
        [TestMethod]
        public void AddSomeDifferentOrderCondition_GetOne_MustMaxOrderItem()
        {
            var eng = new ComicEngine();
            eng.Add(new DataCondition { Order = 2, ConditionFunc=_=>true });
            eng.Add(new DataCondition { Order = 22, ConditionFunc = _ => true });
            eng.Add(new DataCondition { Order = 411, ConditionFunc = _ => true });
            eng.Add(new DataCondition { Order = -12312, ConditionFunc = _ => true });
            Assert.AreEqual(411, eng[0].Order);
            Assert.AreEqual(-12312, eng[eng.Count-1].Order);
            var type =eng.GetComicSourceProviderType("http://www.baidu.com");
            Assert.AreEqual(type.Order, 411);
        }
    }
}
