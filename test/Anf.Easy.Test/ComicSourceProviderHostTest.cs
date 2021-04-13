using Anf.Easy.Test.Visiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class ComicSourceProviderHostTest
    {
        [TestMethod]
        public async Task CallProx_AllMethodWasCalled()
        {
            var ss = new NullServiceScope();
            var provider = new NullSourceProvider();
            var host = new ComicSourceProviderHost(provider,ss);
            Assert.AreEqual(provider, host.ComicSourceProvider);
            Assert.AreEqual(ss, host.Scope);
            await host.GetChaptersAsync(string.Empty);
            await host.GetImageStreamAsync(string.Empty);
            await host.GetPagesAsync(string.Empty);
            Assert.IsTrue(provider.GetImageStreamAsyncVal);
            Assert.IsTrue(provider.GetPagesAsyncVal);
            Assert.IsTrue(provider.GetChaptersAsyncVal);
            host.Dispose();
        }
    }
}
