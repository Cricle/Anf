using Anf.Easy.Test.Provider;
using Anf.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    [TestClass]
    public class ComicVisitingTest
    {
        [TestMethod]
        public void GivenNullValueInit_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ComicVisiting<Stream>(null, StreamResourceFactoryCreator.Default));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicVisiting<Stream>(new NullServiceProvider(), null));
            Assert.ThrowsException<ArgumentNullException>(() => new ComicVisiting<Stream>(null, null));
        }
        private ComicVisiting<Stream> CreateNullVisitor()
        {
            var creator = new StreamResourceFactoryCreator();
            var provider = new NullServiceProvider();
            var visit = new ComicVisiting<Stream>(provider, creator);
            return visit;
        }

        [TestMethod]
        public void GivenValueInit_PropertyMustEqualGiven()
        {
            var creator = new StreamResourceFactoryCreator();
            var provider = new NullServiceProvider();
            var visit = new ComicVisiting<Stream>(provider, creator);
            Assert.AreEqual(creator, visit.ResourceFactoryCreator);
            Assert.AreEqual(provider,visit.Host);
            visit.Dispose();
        }
        [TestMethod]
        public async Task GivenNullOrEmptyAddress_MustThrowException()
        {
            var visitor = CreateNullVisitor();
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => visitor.LoadAsync(null));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => visitor.LoadAsync(string.Empty));
            visitor.Dispose();
        }
        
        [TestMethod]
        public async Task GivenProvider_Load_MustLoadSucceed()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            var prov = visit.Host.GetRequiredService<ResourceComicProvider>();
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            Assert.IsTrue(res);
            Assert.IsNotNull(visit.Entity);
            Assert.IsNotNull(visit.ChapterWithPages);
            Assert.AreEqual(ComicVisitingHelper.AnyUri.AbsoluteUri, visit.Address);
            Assert.IsNotNull(visit.SourceProvider);
            visit.Dispose();
        }
        [TestMethod]
        public async Task GivenNothingProvider_Load_MustLoadFail()
        {
            var creator = new StreamResourceFactoryCreator();
            var eng = new ComicEngine();
            var provider = new ValueServiceProvider
            {
                ServiceMap = new Dictionary<Type, Func<object>>
                {
                    [typeof(ComicEngine)] = () => eng,
                }
            };
            provider.ServiceMap.Add(typeof(IServiceScopeFactory), () => new ValueServiceScopeFactory { ScopeFactory = () => new ValueServiceScope {  ServiceProvider = provider } });
            var visit = new ComicVisiting<Stream>(provider, creator);
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            Assert.IsFalse(res);
            visit.Dispose();
        }
        [TestMethod]
        public async Task GivenProvider_Load_LoadingAndLoadedEventMustFired()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            object sender1 = null;
            string addr1 = null;
            visit.Loading += (o, e) =>
            {
                sender1 = o;
                addr1 = e;
            };
            object sender2 = null;
            ComicEntity entity = null;
            visit.Loaded += (o, e) =>
            {
                sender2 = o;
                entity = e;
            };
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            Assert.AreEqual(visit, sender1);
            Assert.AreEqual(ComicVisitingHelper.AnyUri.AbsoluteUri, addr1);

            Assert.AreEqual(visit, sender2);
            Assert.AreEqual(visit.Entity, entity);
            visit.Dispose();
        }
        [TestMethod]
        public async Task GetChapterWhenNotLoad_OrOutOfRange_MustThrowException()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => visit.GetChapterManagerAsync(0));
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => visit.GetChapterManagerAsync(-1));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => visit.GetChapterManagerAsync(visit.ChapterWithPages.Count));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => visit.GetChapterManagerAsync(99));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => visit.GetChapterManagerAsync(int.MaxValue));
            visit.Dispose();
        }
        [TestMethod]
        public async Task GetChapter_MustReturnTheChapter_AndLoaded()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            var chp = await visit.GetChapterManagerAsync(0);
            Assert.IsNotNull(chp);
            Assert.IsNotNull(chp.ChapterWithPage);
            Assert.AreEqual(visit.ChapterWithPages[0], chp.ChapterWithPage);
            visit.Dispose();
        }
        [TestMethod]
        public async Task GetChapterTwice_InnerObjectMustEqual()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            var chp1 = await visit.GetChapterManagerAsync(0);
            var chp2 = await visit.GetChapterManagerAsync(0);
            Assert.AreEqual(chp1.ChapterWithPage, chp2.ChapterWithPage);
            visit.Dispose();
        }
        [TestMethod]
        public async Task GetChapter_EventLoadingAndLoadedChapterMustBeFired()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            object sender1 = null;
            int index1 = -1;
            visit.LoadingChapter += (o, e) =>
            {
                sender1 = o;
                index1 = e;
            };
            object sender2 = null;
            ChapterWithPage cwp = null;
            visit.LoadedChapter += (o, e) =>
            {
                sender2 = o;
                cwp = e;
            };
            var chp1 = await visit.GetChapterManagerAsync(0);
            Assert.AreEqual(visit, sender1);
            Assert.AreEqual(0, index1);

            Assert.AreEqual(visit, sender2);
            Assert.AreEqual(visit.ChapterWithPages[0], cwp);

            sender1 = sender2 = null;
            index1 = -1;
            cwp = null;

            chp1 = await visit.GetChapterManagerAsync(0);

            Assert.IsNull(sender1);
            Assert.IsNull(sender2);
            Assert.AreEqual(-1, index1);
            Assert.IsNull(cwp);
            visit.Dispose();
        }
        [TestMethod]
        public void SetNullResourceFactoryCreator_MustThrowException()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            Assert.ThrowsException<ArgumentNullException>(() => visit.ResourceFactoryCreator = null);
            visit.Dispose();
        }
        [TestMethod]
        public async Task LoadOutOufRangeChapter_MustThrowException()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => visit.LoadChapterAsync(-1));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => visit.LoadChapterAsync(visit.ChapterWithPages.Count));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => visit.LoadChapterAsync(20));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(() => visit.LoadChapterAsync(int.MaxValue));
        }
        [TestMethod]
        public async Task LoadChapter_EraseIt_ChapterMustBeErased()
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            var chp1 = await visit.GetChapterManagerAsync(0);
            visit.EraseChapter(0);
            Assert.IsNull(visit.ChapterWithPages[0]);
            chp1 = await visit.GetChapterManagerAsync(0);
            Assert.IsNotNull(chp1);
            Assert.AreEqual(visit.ChapterWithPages[0], chp1.ChapterWithPage);
        }
        [TestMethod]
        public async Task LoadChapter_IfAnyIntercept_TheMethodWasCalled()
        {
            var intercept = new NullComicVisitingInterceptor<Stream>();
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            visit.VisitingInterceptor = intercept;
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            var chp1 = await visit.GetChapterManagerAsync(0);
            Assert.IsTrue(intercept.IsGotChapterManagerAsync);
            Assert.IsTrue(intercept.IsLoadingChapterAsync);
            Assert.IsTrue(intercept.IsLoadedChapterAsync);
        }
        [TestMethod]
        [DataRow(1)]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(20)]
        public async Task AsyncLoadChapter_MustOnlyOneLoad(int size)
        {
            var visit = ComicVisitingHelper.CreateResrouceVisitor();
            var res = await visit.LoadAsync(ComicVisitingHelper.AnyUri.AbsoluteUri);
            var insts = new object[size];
            var tasks = Enumerable.Range(0, size)
                .Select(x => visit.GetChapterManagerAsync(0).ContinueWith(y => insts[x] = y.Result.ChapterWithPage))
                .ToArray();
            await Task.WhenAll(tasks);
            Assert.IsNotNull(visit.ChapterWithPages[0]);
            for (int i = 0; i < insts.Length; i++)
            {
                var val = insts[i];
                Assert.AreEqual(visit.ChapterWithPages[0], val, i.ToString());
            }
        }
    }
}
