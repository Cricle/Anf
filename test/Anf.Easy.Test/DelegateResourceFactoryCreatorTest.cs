using Anf.Easy.Visiting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class DelegateResourceFactoryCreatorTest
    {
        [TestMethod]
        public void InitWithNull_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DelegateResourceFactoryCreator<int>(null));
        }
        [TestMethod]
        public async Task InitWithDelegate_Create_ValueMustCreated()
        {
            var fc = new NullResourceFactory<int>();
            Func<ResourceFactoryCreateContext<int>, Task<IResourceFactory<int>>> del = _ => Task.FromResult<IResourceFactory<int>>(fc);
            var f = new DelegateResourceFactoryCreator<int>(del);
            Assert.AreEqual(del, f.Delegate);
            var v = await f.CreateAsync(null);
            Assert.AreEqual(fc, v);
        }
    }
}
