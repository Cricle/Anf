using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Test.Visiting
{
    class AddSharedComic:SharedComic
    {
        public AddSharedComic(IComicSourceProvider sourceProvider, RecyclableMemoryStreamManager recyclableMemoryStreamManager, int? capacity) 
            : base(sourceProvider, recyclableMemoryStreamManager, capacity)
        {
        }

        protected override Task<Stream> DownloadAsync(string address)
        {
            return Task.FromResult(Stream.Null);
        }
    }
    [TestClass]
    public class SharedComicTest
    {
        [TestMethod]
        public async Task Get2Same_OnlyOne_Loaded()
        {
            var c = new AddSharedComic(null, null, 20);
            await Task.WhenAll(Enumerable.Range(0, 40).Select(x => c.GetOrLoadAsync("a")));
            Assert.AreEqual(1, c.Streams.Count);
        }
    }
}
