using Anf.Test.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test
{
    [TestClass]
    public class ComicSourceConditionBaseTest
    {
        [TestMethod]
        public void GivenBindTypeTypeInit_ProviderTypeMustEqualBindType()
        {
            var condition = new DataProviderComicSourceCondition();
            Assert.AreEqual(typeof(DataProvider), condition.ProviderType);
        }
    }
}
