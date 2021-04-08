using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test
{
    [TestClass]
    public class ComicSourceContextTest
    {
        [TestMethod]
        public void GivenNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentException>(() => new ComicSourceContext((string)null));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicSourceContext((Uri)null));
        }
        [TestMethod]
        public void GivenValue_PropertyMustEquipGiven()
        {
            var uri = new Uri("http://www.bing.com/");
            var ctx = new ComicSourceContext(uri);
            Assert.AreEqual(uri, ctx.Uri);
            Assert.AreEqual(uri.AbsoluteUri, ctx.Source);
            ctx = new ComicSourceContext(uri.AbsoluteUri);
            Assert.AreEqual(uri, ctx.Uri);
            Assert.AreEqual(uri.AbsoluteUri, ctx.Source);
            var str = "-no-uri-";
            ctx = new ComicSourceContext(str);
            Assert.AreEqual(str, ctx.Source);
        }
    }
}
