using Anf.Networks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test.Networks
{
    [TestClass]
    public class WebRequestAdapterTest
    {
        [TestMethod]
        public async Task GivenNullValue_MustThrowException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => new WebRequestAdapter().GetStreamAsync(null));
        }
    }
}
