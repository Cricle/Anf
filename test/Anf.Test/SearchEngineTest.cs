using Anf.Test.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test
{
    [TestClass]
    public class SearchEngineTest
    {
        [TestMethod]
        public void InitWithNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SearchEngine(null));
        }
        [TestMethod]
        public void InitWithNotNullValue_MustGotValue()
        {
            var inst = new NullServiceScopeFactory();
            var se = new SearchEngine(inst);
            Assert.AreEqual(inst, se.ServiceScopeFactory);
        }
        private SearchEngine CreateEng()
        {
            var factory = new ValueServiceScopeFactory();
            factory.Factory = new Dictionary<Type, Func<object>>
            {
                [typeof(DataSearchProvider)] = () => new DataSearchProvider
                {
                    Datas = new Dictionary<string, SearchComicResult>
                    {
                        ["aaa"] = new SearchComicResult
                        {
                            Snapshots = new ComicSnapshot[]
                                {
                                   new ComicSnapshot
                                   {
                                       Name="aaa",
                                        Author="aa",
                                         Descript="aaaaa",
                                          ImageUri="aaaaaaaaa",
                                           Sources=new ComicSource[]
                                           {
                                               new ComicSource{  Name="ss1", TargetUrl="ssurl"},
                                               new ComicSource(),
                                           },
                                            TargetUrl="aaaaaaaaaa"
                                   },
                                   new ComicSnapshot()
                                },
                            Support = true,
                            Total = 2
                        }
                    }
                },
                [typeof(DataSearchProvider2)] = () => new DataSearchProvider
                {
                    Datas = new Dictionary<string, SearchComicResult>
                    {
                        ["bbb"] = new SearchComicResult
                        {
                            Snapshots = new ComicSnapshot[]
                                {
                                   new ComicSnapshot
                                   {
                                       Name="bbb",
                                       Author="bb",
                                       Descript="aaabbbaa",
                                       ImageUri="bb",
                                       Sources=new ComicSource[]
                                       {
                                            new ComicSource{  Name="ss1", TargetUrl="ssurl"},
                                            new ComicSource(),
                                       },
                                       TargetUrl="bbbbbbb"
                                   },
                                   new ComicSnapshot()
                                },
                            Support = true,
                            Total = 11
                        }
                    }
                }
            };
            var eng = new SearchEngine(factory);
            eng.Add(typeof(DataSearchProvider));
            eng.Add(typeof(DataSearchProvider2));
            return eng;
        }
        [TestMethod]
        public async Task RunSearch_MustToEachSearch()
        {
            var eng = CreateEng();
            var res = await eng.SearchAsync("aaa", 11, 11);
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Snapshots);
            Assert.IsTrue(res.Snapshots.Length > 0);
            Assert.IsTrue(res.Support);
            Assert.AreEqual(2, res.Total);
            Assert.AreEqual("aaa", res.Snapshots[0].Name);
            res = await eng.SearchAsync("bbb", 11, 11);
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Snapshots);
            Assert.IsTrue(res.Snapshots.Length > 0);
            Assert.IsTrue(res.Support);
            Assert.AreEqual(11, res.Total);
            Assert.AreEqual("bbb", res.Snapshots[0].Name);
            res = await eng.SearchAsync("ccc", 11, 11);
            Assert.AreEqual(0, res.Snapshots.Length);
            Assert.IsFalse(res.Support);
            Assert.AreEqual(0, res.Total);
        }
        [TestMethod]
        public async Task GetSeachCursor_MustReturnCursor()
        {
            var eng = CreateEng();
            var cur = await eng.GetSearchCursorAsync("aaa", 1, 10);
            Assert.AreEqual("aaa", cur.Keyword);
            Assert.AreEqual(10, cur.Take);
            var a = await cur.MoveNextAsync();
            Assert.IsFalse(a);
            Assert.IsNotNull(a);
            cur.Dispose();
        }
    }
}
