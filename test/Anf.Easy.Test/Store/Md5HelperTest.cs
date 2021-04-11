using Anf.Easy.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test.Store
{
    [TestClass]
    public class Md5HelperTest
    {
        [TestMethod]
        public void GivenStringToMakeMd5_TwiceMustEqual()
        {
            var str = "627135764sjhavcxhsjkhsda";
            var str1 = Md5Helper.MakeMd5(str);
            var str2= Md5Helper.MakeMd5(str);
            Assert.AreEqual(str1, str2);
        }
        [TestMethod]
        public void GivenEncodingAndStringToMakeMd5_TwiceMustEqual()
        {
            var str = "627135764sjhavcxhsjkhsda";
            var str1 = Md5Helper.MakeMd5(str,Encoding.UTF8);
            var str2 = Md5Helper.MakeMd5(str, Encoding.UTF8);
            Assert.AreEqual(str1, str2);
        }
    }
}
