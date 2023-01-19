using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test
{
    [TestClass]
    public class DownloadExceptionListenerContextTest
    {
        [TestMethod]
        [DataRow(typeof(Exception))]
        [DataRow(typeof(ArgumentException))]
        [DataRow(typeof(InvalidOperationException))]
        [DataRow(typeof(InvalidProgramException))]
        public void GivenException_PropertyValueMustEqualGiven(Type type)
        {
            var ex = (Exception)Activator.CreateInstance(type);
            var ctx = new DownloadExceptionListenerContext(null, null, null, default, ex);
            Assert.AreEqual(ex, ctx.Exception);
        }
    }
}
