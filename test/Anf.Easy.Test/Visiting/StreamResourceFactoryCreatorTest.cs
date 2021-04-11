using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class NullComicVisiting<T> : IComicVisiting<T>
    {
        public IComicVisitingInterceptor<T> VisitingInterceptor { get; set; }
        public IResourceFactoryCreator<T> ResourceFactoryCreator { get; set; }

        public IResourceFactory<T> ResourceFactory { get; set; }

        public IComicSourceProvider SourceProvider { get; set; }

        public string Address { get; set; }

        public IServiceProvider Host { get; set; }

        public ComicEntity Entity { get; set; }

        public event Action<ComicVisiting<T>, string> Loading;
        public event Action<ComicVisiting<T>, ComicEntity> Loaded;
        public event Action<ComicVisiting<T>, int> LoadingChapter;
        public event Action<ComicVisiting<T>, ChapterWithPage> LoadedChapter;

        public void Dispose()
        {
        }

        public void EraseChapter(int index)
        {
        }

        public Task<IComicChapterManager<T>> GetChapterManagerAsync(int index)
        {
            return Task.FromResult<IComicChapterManager<T>>(null);
        }

        public Task<bool> LoadAsync(string address)
        {
            return Task.FromResult(true);
        }

        public Task LoadChapterAsync(int index)
        {
            return Task.FromResult(0);
        }
    }
    [TestClass]
    public class StreamResourceFactoryCreatorTest
    {
        [TestMethod]
        public async Task Init_CallGetCreate_MustReturnNotNull()
        {
            var factor = new StreamResourceFactoryCreator();
            var prov = new NullSourceProvider();
            var addr = "sdasafsd";
            var visit = new NullComicVisiting<Stream>();
            var ctx = new ResourceFactoryCreateContext<Stream>
            {
                SourceProvider = prov,
                Address = addr,
                Visiting = visit
            };
            Assert.AreEqual(prov, ctx.SourceProvider);
            Assert.AreEqual(addr, ctx.Address);
            Assert.AreEqual(visit, ctx.Visiting);
            Assert.AreEqual(visit.Host, ctx.ServiceProvider);
            var c = await factor.CreateAsync(ctx);
            Assert.IsNotNull(c);
            c.Dispose();
        }
        [TestMethod]
        public void GetDefault_TwiceMustEqual()
        {
            var a = StreamResourceFactoryCreator.Default;
            var b = StreamResourceFactoryCreator.Default;
            Assert.AreEqual(a, b);
        }
    }
}
