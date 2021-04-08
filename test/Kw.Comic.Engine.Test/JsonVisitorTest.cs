using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anf.Test
{
    [TestClass]
    public class JsonVisitorTest
    {
        [TestMethod]
        public void GivenNull_MustException()
        {
            Assert.ThrowsException<ArgumentNullException>(()=>new JsonVisitor(null));
        }
        [TestMethod]
        public void GivenObject_GetProperty_MustGotValue()
        {
            var obj = JObject.FromObject(new
            {
                Name = "Joke",
                Email = "joke@well.com"
            });
            var visitor = new JsonVisitor(obj);
            var val = visitor["Name"];
            Assert.AreEqual("Joke", val.ToString());
        }
        [TestMethod]
        public void GivenArray_GetProperty_MustGotArray()
        {
            var obj = JArray.FromObject(new[] { 1,2,3,4,5,6});
            var visitor = new JsonVisitor(obj);
            var arr = visitor.ToArray();
            var count = arr.Count();
            Assert.AreEqual(obj.Count, count);
            Assert.AreEqual("1", arr.First().ToString());
        }
        [TestMethod]
        public void GivenObject_GetPropertyWithInnder_MustGotValue()
        {
            var obj = JObject.FromObject(new
            {
                Car = new
                {
                    Owner="Joke"
                }
            });
            var visitor = new JsonVisitor(obj);
            var value = visitor["Car"]["Owner"].ToString();
            Assert.AreEqual("Joke", value);
        }
        [TestMethod]
        public void GivenObject_DisposeIt()
        {
            var obj = new JObject();
            var visitor = new JsonVisitor(obj);
            visitor.Dispose();
        }
    }
}
