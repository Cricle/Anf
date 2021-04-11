using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class NullComicVisitingInterceptor<T> : IComicVisitingInterceptor<T>
    {
        public bool IsGettingPageAsync { get; set; }
        public bool IsGotChapterManagerAsync { get; set; }
        public bool IsGotPageAsync { get; set; }
        public bool IsLoadedChapterAsync { get; set; }
        public bool IsLoadedComicAsync { get; set; }
        public bool IsLoadingChapterAsync { get; set; }
        public bool IsLoadingComicAsync { get; set; }
        public Task GettingPageAsync(GettingPageInterceptorContext<T> context)
        {
            IsGettingPageAsync = true;
            return Task.FromResult(0);
        }

        public Task GotChapterManagerAsync(GotChapterManagerInterceptorContext<T> context)
        {
            IsGotChapterManagerAsync = true;
            return Task.FromResult(0);
        }

        public Task GotPageAsync(GettingPageInterceptorContext<T> context)
        {
            IsGotPageAsync = true;
            return Task.FromResult(0);
        }

        public Task LoadedChapterAsync(ChapterVisitingInterceptorContext<T> context)
        {
            IsLoadedChapterAsync = true;
            return Task.FromResult(0);
        }

        public Task LoadedComicAsync(LoadVisitingInterceptorContext<T> context)
        {
            IsLoadedComicAsync = true;
            return Task.FromResult(0);
        }

        public Task LoadingChapterAsync(ChapteringVisitingInterceptorContext<T> context)
        {
            IsLoadingChapterAsync = true;
            return Task.FromResult(0);
        }

        public Task LoadingComicAsync(LoadVisitingInterceptorContext<T> context)
        {
            IsLoadingComicAsync = true;
            return Task.FromResult(0);
        }
    }
    internal class NullResourceFactory<T> : IResourceFactory<T>
    {
        public void Dispose()
        {
        }

        public Dictionary<string,T> Values { get; set; }

        public Task<T> GetAsync(string address)
        {
            return Task.FromResult(Values[address]);
        }
    }
    [TestClass]
    public class ComicChapterManagerTest
    {
        private ComicVisiting<T> CreateVisiting<T>(NullResourceFactory<T> factory=null)
        {
            return new ComicVisiting<T>(new NullServiceProvider(), new DelegateResourceFactoryCreator<T>(_ => Task.FromResult<IResourceFactory<T>>(factory)));
        }
        [TestMethod]
        public void GivenNullInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ComicChapterManager<object>(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicChapterManager<object>(new ChapterWithPage(null,null), null));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicChapterManager<object>(null, CreateVisiting<object>()));
        }
        [TestMethod]
        public void GivenValueInit_PropertyValueMustEqualGiven()
        {
            var page = new ChapterWithPage(null, null);
            var visit = CreateVisiting<object>();
            var mgr = new ComicChapterManager<object>(page, visit);
            Assert.AreEqual(page, mgr.ChapterWithPage);
            Assert.AreEqual(visit, mgr.ComicVisiting);
        }
        [TestMethod]
        public async Task GivenValueInit_GetVisitPage_MustCallIntercepts()
        {
            var addr = "dsasafsa";
            var page = new ChapterWithPage(null, new ComicPage[]
            {
                new ComicPage{ TargetUrl=addr}
            });
            var f = new NullResourceFactory<object>();
            var obj = new object();
            f.Values = new Dictionary<string, object>
            {
                [addr] = obj
            };
            var visit = CreateVisiting<object>(f);
            var intercept = new NullComicVisitingInterceptor<object>();
            visit.VisitingInterceptor = intercept;
            var mgr = new ComicChapterManager<object>(page, visit);
            var pg = await mgr.GetVisitPageAsync(0);
            Assert.AreEqual(obj, pg.Resource);
            Assert.AreEqual(page.Pages[0], pg.Page);

            Assert.IsTrue(intercept.IsGettingPageAsync);
            Assert.IsTrue(intercept.IsGotPageAsync);
        }
    }
}
