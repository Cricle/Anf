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
            var name = "-name-";
            var url = new Uri("http://localhost:5000");
            var faUrl = new Uri("http://localhost:5001");
            var cd = true;
            var condition = new DataProviderComicSourceCondition(name, url, faUrl, _ => cd);
            Assert.AreEqual(typeof(DataProvider), condition.ProviderType);
            Assert.AreEqual(name, condition.EngineName);
            Assert.AreEqual(url, condition.Address);
            Assert.AreEqual(faUrl, condition.FaviconAddress);
            Assert.AreEqual(cd, condition.Condition(null));
        }
    }
}
