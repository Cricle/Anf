using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test
{
    [TestClass]
    public class SearchEngineTest
    {
        [TestMethod]
        public void InitWithNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SearchEngine(null));
        }
        [TestMethod]
        public void InitWithNotNullValue_MustGotValue()
        {
            new SearchEngine(new NullServiceScopeFactory());
        }
    }
}
