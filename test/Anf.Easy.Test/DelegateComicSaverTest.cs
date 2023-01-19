using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    [TestClass]
    public class DelegateComicSaverTest
    {
        [TestMethod]
        public void GivenDelegate_CallMethod_DelegateMustBeCalled()
        {
            var ok1 = false;
            var ok2 = false;
            var saver = new DelegateComicSaver(_ => ok1 = true, _ =>
            {
                ok2 = true;
                return Task.FromResult(0);
            });
            saver.NeedToSave(default);
            saver.SaveAsync(default);
            Assert.IsTrue(ok1);
            Assert.IsTrue(ok2);

            ok1 = ok2 = false;

            saver = new DelegateComicSaver( _ =>
            {
                ok2 = true;
                return Task.FromResult(0);
            });
            saver.NeedToSave(default);
            saver.SaveAsync(default);
            Assert.IsTrue(ok2);
        }
        [TestMethod]
        public void GivenNullValue_MustThrowException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DelegateComicSaver(null));
            Assert.ThrowsException<ArgumentNullException>(() => new DelegateComicSaver(null, _ => Task.FromResult(1)));
        }
    }
}
