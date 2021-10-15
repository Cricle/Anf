using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Test
{
    [TestClass]
    public class PlatformResourceCreatorFactoryTest
    {
        class NullComicSourceProvider : IComicSourceProvider
        {
            public Task<ComicEntity> GetChaptersAsync(string targetUrl)
            {
                return Task.FromResult<ComicEntity>(null);
            }

            public Task<Stream> GetImageStreamAsync(string targetUrl)
            {
                return Task.FromResult<Stream>(null);
            }

            public Task<ComicPage[]> GetPagesAsync(string targetUrl)
            {
                return Task.FromResult<ComicPage[]>(null);
            }
        }
        [TestMethod]
        public async Task CreateFromFactory()
        {
            var provider = new NullComicSourceProvider();
            var fc = new PlatformResourceCreatorFactory<object, object>();
            var ctx = new ResourceFactoryCreateContext<object>
            {
                SourceProvider = provider
            };
            var val = await fc.CreateAsync(ctx);
            Assert.IsNotNull(val);
            Assert.AreEqual(fc.EnableCache, ((PlatformResourceCreator<object,object>)val).EnableCache);

            fc.EnableCache = false;
            val = await fc.CreateAsync(ctx);
            Assert.IsNotNull(val);
            Assert.AreEqual(fc.EnableCache, ((PlatformResourceCreator<object, object>)val).EnableCache);

        }
    }
}
