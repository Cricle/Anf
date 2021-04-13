using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class PageBoxTest
    {
        [TestMethod]
        public void GivenAnyPropertyValue_GetMustEqualGiven()
        {
            var page = new ComicPage();
            var resource = new object();
            var box = new PageBox<object>();
            box.DoNotDispose = true;
            box.Page = page;
            box.Resource = resource;
            Assert.IsTrue(box.DoNotDispose);
            Assert.AreEqual(page, box.Page);
            Assert.AreEqual(resource, box.Resource);
        }
        [TestMethod]
        public void GivenDisposableValue_DisposeIt_MustBeDisposed()
        {
            var res = new DispoableObject();
            var box = new PageBox<object>();
            box.DoNotDispose = false;
            box.Resource = res;
            box.Dispose();
            Assert.IsTrue(res.IsDisposed);
        }
        [TestMethod]
        public void GivenDisposableValue_SetDoNotDispose_DisposeIt_MustBeDisposed()
        {
            var res = new DispoableObject();
            var box = new PageBox<object>();
            box.DoNotDispose = true;
            box.Resource = res;
            box.Dispose();
            Assert.IsFalse(res.IsDisposed);
        }
    }
}
