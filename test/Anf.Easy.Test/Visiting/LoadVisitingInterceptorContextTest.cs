using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class LoadVisitingInterceptorContextTest
    {
        [TestMethod]
        public void GivenValue_PropertyMustEqualInput()
        {
            var ctx = new LoadVisitingInterceptorContext<object>();
            Assert.IsFalse(ctx.IsSwitch);
            ctx.IsSwitch = true;
            Assert.IsTrue(ctx.IsSwitch);
            var addr = "-addr-";
            Assert.IsNull(ctx.Address);
            ctx.Address = addr;
            Assert.AreEqual(addr, ctx.Address);
        }
    }
}
