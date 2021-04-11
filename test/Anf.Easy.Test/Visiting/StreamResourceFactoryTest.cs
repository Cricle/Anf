using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class StreamResourceFactoryTest
    {
        [TestMethod]
        public async Task CallGetAsync_ProviderGetImageStreamAsync_MustBecalled()
        {
            var provider = new NullSourceProvider();
            var f = new StreamResourceFactory(provider);
            await f.GetAsync(null);
            Assert.IsTrue(provider.GetImageStreamAsyncVal);
            f.Dispose();
        }
        [TestMethod]
        public void GivenNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new StreamResourceFactory(null));
        }
    }
}
