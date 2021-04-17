using Anf.Test.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test
{
    [TestClass]
    public class ComicSourceProviderExtenionsTest
    {
        [TestMethod]
        public async Task GotWithUrl_MustReturnEntity()
        {
            var addr = "http://www.bing.com/a";
            var source = new DataProvider
            {
                Entity = new Dictionary<string, ComicEntity>
                {
                    [addr] = new ComicEntity
                    {
                        Chapters = new ComicChapter[]
                          {
                              new ComicChapter{ TargetUrl=addr+1},
                              new ComicChapter{ TargetUrl=addr+2},
                              new ComicChapter{ TargetUrl=addr+3},
                          }
                    }
                },
                Pages = new Dictionary<string, ComicPage[]>
                {
                    [addr + 1] = new ComicPage[0],
                    [addr + 2] =new ComicPage[0],
                    [addr + 3] = new ComicPage[] { new ComicPage() },
                }
            };
            var provider = new NullChapterAnalysisNotifyer();
            var detail = await source.GetChapterWithPageAsync(addr, provider);
            Assert.IsNotNull(detail);
            Assert.AreEqual(3, detail.Chapters.Length);
            Assert.IsNotNull(detail.Entity);
            for (int i = 0; i < detail.Chapters.Length; i++)
            {
                Assert.IsNotNull(detail.Chapters[i]?.Chapter);
                Assert.IsNotNull(detail.Chapters[i]?.Pages);
            }
        }
    }
}
