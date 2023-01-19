using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class DelegateResourceFactoryTest
    {
        [TestMethod]
        public void InitWithNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>new DelegateResourceFactory<int>(null));
        }
        [TestMethod]
        public async Task InitWithDelegate_Create_ValueMustCreated()
        {
            var fc = new NullResourceFactory<int>();
            Func<ResourceFactoryCreateContext<int>, Task<IResourceFactory<int>>> del = _ => Task.FromResult<IResourceFactory<int>>(fc);
            var f = new DelegateResourceFactory<int>(del);
            Assert.AreEqual(del, f.Creator);
            var v=await f.CreateAsync(null);
            Assert.AreEqual(fc, v);
        }
    }
}
