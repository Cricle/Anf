using Anf.Easy.Test.Visiting;
using Anf.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class ComicHostExtensionsTest
    {
        [TestMethod]
        public async Task GivenNullValue_CallGetVisitingAndLoadAsync_MustThrowExcetpion()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => ComicHostExtensions.GetVisitingAndLoadAsync<Stream>(null, "-any-"));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => ComicHostExtensions.GetVisitingAndLoadAsync<Stream>(new NullServiceProvider(), null));
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => ComicHostExtensions.GetVisitingAndLoadAsync<Stream>(new NullServiceProvider(), string.Empty));
        }
        [TestMethod]
        public async Task GetVisitingAndLoadAsync_MustReturnLoadedVisitor()
        {
            var provider = new ValueServiceProvider
            {
                 ServiceMap=new Dictionary<Type, Func<object>>
                 {
                     [typeof(IComicVisiting<Stream>)]=()=>ComicVisitingHelper.CreateResrouceVisitor()
                 }
            };
            var visitor=await ComicHostExtensions.GetVisitingAndLoadAsync<Stream>(provider, "http://localhost:8765");
            Assert.IsNotNull(visitor);
            Assert.IsNotNull(visitor.Entity);
        }
        [TestMethod]
        public async Task GetVisitingAndLoadAsync_FailToLoad_MustReturnNull()
        {
            var provider = new ValueServiceProvider
            {
                ServiceMap = new Dictionary<Type, Func<object>>
                {
                    [typeof(IComicVisiting<Stream>)] = () => new NullComicVisiting<Stream> { LoadSucceed=false}
                }
            };
            var visitor = await ComicHostExtensions.GetVisitingAndLoadAsync<Stream>(provider, "http://localhost:8765");
            Assert.IsNull(visitor);
        }
        [TestMethod]
        public void GivenNullValue_CallGetServiceScope_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ComicHostExtensions.GetServiceScope(null));
        }
        [TestMethod]
        public void CallGetServiceScope_MustReturnScopeInstance()
        {
            var scope = new NullServiceScope();
            var provider = new ValueServiceProvider
            {
                ServiceMap = new Dictionary<Type, Func<object>>
                {
                    [typeof(IServiceScopeFactory)] =()=>new ValueServiceScopeFactory { ScopeFactory=()=> scope }
                }
            };

            var retScope = ComicHostExtensions.GetServiceScope(provider);
            Assert.AreEqual(scope, retScope);
        }
        [TestMethod]
        public void GivenNullValue_CallGetComicEngine_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ComicHostExtensions.GetComicEngine(null));
        }
        [TestMethod]
        public void CallGetComicEngine_MustReturnEngineInstance()
        {
            var eng = new ComicEngine();
            var provider = new ValueServiceProvider
            {
                ServiceMap = new Dictionary<Type, Func<object>>
                {
                    [typeof(ComicEngine)] = () => eng
                }
            };
            var retEng = ComicHostExtensions.GetComicEngine(provider);
            Assert.AreEqual(eng, retEng);
        }
        [TestMethod]
        public void GivenNullValue_CallGetSeachEngine_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ComicHostExtensions.GetSearchEngine(null));
        }
        [TestMethod]
        public void CallGetSearchEngine_MustReturnEngineInstance()
        {
            var eng = new SearchEngine(new ValueServiceScopeFactory());
            var comicEng = new ComicEngine();
            var provider = new ValueServiceProvider
            {
                ServiceMap = new Dictionary<Type, Func<object>>
                {
                    [typeof(SearchEngine)] = () => eng,
                    [typeof(ComicEngine)] = () => comicEng
                }
            };
            var retEng = ComicHostExtensions.GetComicEngine(provider);
            Assert.AreEqual(comicEng, retEng);
        }
        [TestMethod]
        public async Task GivenNullValue_CallSearchAsync_MustThrowException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => ComicHostExtensions.SearchAsync(null,null,0,0));
        }
        [TestMethod]
        public async Task SearchAsync_MustReturnSearchResult()
        {
            var provider = new ValueServiceProvider
            {
                ServiceMap = new Dictionary<Type, Func<object>>
                {
                    
                }
            };
            var provfc = new ValueServiceScopeFactory { ScopeFactory = () => new ValueServiceScope { ServiceProvider = provider } };
            var eng = new SearchEngine(provfc);
            provider.ServiceMap[typeof(SearchEngine)] = () => eng;
            provider.ServiceMap
                    [typeof(IServiceScopeFactory)] = () => provfc;
            var res = await ComicHostExtensions.SearchAsync(provider, null, 5, 10);
            Assert.IsNotNull(res);
        }

    }
}
