using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Test
{
    [TestClass]
    public class UrlHelperTest
    {
        [TestMethod]
        [DataRow("http://www.bing.com")]
        [DataRow("http://www.bing.com/")]
        [DataRow("www.bing.com/")]
        [DataRow("www.bing.com")]
        [DataRow("www.bing.com?a=111")]
        [DataRow("www.bing.com/a/123")]
        [DataRow("http://www.bing.com/a/123")]
        [DataRow("http://www.bing.com?a=123")]
        [DataRow("https://www.bing.com?a=123")]
        [DataRow("https://www.bing.com/a/123")]
        [DataRow("https://www.bing.com")]
        [DataRow("https://www.bing.com/")]
        public void FastGetHost_MustReturnUrlHost(string url)
        {
            var val = UrlHelper.FastGetHost(url);
            Assert.AreEqual("www.bing.com", val);
        }
        [TestMethod]
        [DataRow("https://www.aaa.xxx.bing.com/")]
        [DataRow("www.aaa.xxx.bing.com/")]
        [DataRow("www.aaa.xxx.bing.com")]
        [DataRow("www.aaa.xxx.bing.com?a=123")]
        [DataRow("www.aaa.xxx.bing.com/123/2312")]
        [DataRow("http://www.aaa.xxx.bing.com/")]
        [DataRow("http://www.aaa.xxx.bing.com")]
        [DataRow("http://www.aaa.xxx.bing.com/123/aaa")]
        [DataRow("http://www.aaa.xxx.bing.com?a=123&b=aaa")]
        public void FastGetHostWithPart_MustReturnUrlHostWithPart(string url)
        {
            var val = UrlHelper.FastGetHost(url);

            Assert.AreEqual("www.aaa.xxx.bing.com", val);
        }
        [TestMethod]
        [DataRow("https://www.aaa.xxx.bing.com/")]
        [DataRow("www.aaa.xxx.bing.com/")]
        [DataRow("www.aaa.xxx.bing.com")]
        [DataRow("www.aaa.xxx.bing.com?a=123")]
        [DataRow("www.aaa.xxx.bing.com/123/2312")]
        [DataRow("http://www.aaa.xxx.bing.com/")]
        [DataRow("http://www.aaa.xxx.bing.com")]
        [DataRow("http://www.aaa.xxx.bing.com/123/aaa")]
        [DataRow("http://www.aaa.xxx.bing.com?a=123&b=aaa")]
        public void GivenHttpUrl_MustReturnTrue(string url)
        {
            var res = url.IsWebsite();
            Assert.IsTrue(res);
        }
        [TestMethod]
        [DataRow("--no-url")]
        [DataRow("dsads")]
        [DataRow("ww.xs")]
        [DataRow("http:www")]
        [DataRow("https:www")]
        public void GivenNoHttpUrl_MustReturnFalse(string url)
        {
            var res = url.IsWebsite();
            Assert.IsFalse(res);
        }
    }
}
