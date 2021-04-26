using Anf.Test.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test
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
        public void GetNothingMatch_MustReturnNull()
        {
            var eng = new ComicEngine();
            var val = eng.GetComicSourceProviderType("http://www.baidu.com");
            Assert.IsNull(val);
            eng.Add(new DataCondition { Descript=new EngineDescript(), EngineName="test", Order=1, ProviderType=typeof(void), ConditionFunc = _ => false });
            val = eng.GetComicSourceProviderType(new Uri("http://www.baidu.com"));
            Assert.IsNull(val);
        }
        [TestMethod]
        public void GetWhenException_MustReturnNull()
        {
            var eng = new ComicEngine();
            eng.Add(new DataCondition { ConditionFunc = _ => throw new Exception() });
            var val= eng.GetComicSourceProviderType(new Uri("http://www.baidu.com"));
            Assert.IsNull(val);
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
