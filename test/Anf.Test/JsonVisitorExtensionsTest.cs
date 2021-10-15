using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test
{
    [TestClass]
    public class JsonVisitorExtensionsTest
    {
        [TestMethod]
        public void GivenNullCall_MustThrowException()
        {
            var visitor = new JsonVisitor();
            var type = typeof(object);
            Assert.ThrowsException<ArgumentNullException>(() => JsonVisitorExtensions.TryGet(null, type, out _));
            Assert.ThrowsException<ArgumentNullException>(() => JsonVisitorExtensions.TryGet(visitor, null, out _));
            Assert.ThrowsException<ArgumentNullException>(() => JsonVisitorExtensions.TryGet<object>(null, out _));
            Assert.ThrowsException<ArgumentNullException>(() => JsonVisitorExtensions.TryGet<object>(null, out _, out _));
            Assert.ThrowsException<ArgumentNullException>(() => JsonVisitorExtensions.TryGet(null, typeof(int), out _, out _));
            Assert.ThrowsException<ArgumentNullException>(() => JsonVisitorExtensions.TryGet(visitor, null, out _, out _));
        }
        enum Colors
        {
            Red=1,
            Blue=2
        }
        [TestMethod]
        public void GivenPrimaryType_Get_MustParsedValue()
        {
            var visitor = new JsonVisitor("1");
            var val = JsonVisitorExtensions.TryGet(visitor, typeof(int), out var res);
            Assert.IsTrue(val);
            Assert.AreEqual(1, res);

            visitor = new JsonVisitor("1.123");
            val = JsonVisitorExtensions.TryGet(visitor, typeof(double), out res);
            Assert.IsTrue(val);
            Assert.AreEqual(1.123d, res);

            visitor = new JsonVisitor("1.123");
            val = JsonVisitorExtensions.TryGet<double>(visitor, out var dres);
            Assert.IsTrue(val);
            Assert.AreEqual(1.123d, dres);

            visitor = new JsonVisitor("1.123");
            val = JsonVisitorExtensions.TryGet<double?>(visitor, out var ndres);
            Assert.IsTrue(val);
            Assert.AreEqual(1.123d, ndres.Value);

            visitor = new JsonVisitor("1");
            val = JsonVisitorExtensions.TryGet<Colors>(visitor, out var color);
            Assert.IsTrue(val);
            Assert.AreEqual(Colors.Red, color);

        }
        [TestMethod]
        public void GivenCannotConvertValue_MustPaseFail()
        {
            var visitor = new JsonVisitor("a");
            var val = JsonVisitorExtensions.TryGet(visitor, typeof(int), out var res);
            Assert.IsFalse(val);

            visitor = new JsonVisitor("a");
            val = JsonVisitorExtensions.TryGet<int>(visitor, out _);
            Assert.IsFalse(val);

            visitor = new JsonVisitor("a");
            val = JsonVisitorExtensions.TryGet<object>(visitor, out _);
            Assert.IsFalse(val);

            visitor = new JsonVisitor("a");
            val = JsonVisitorExtensions.TryGet<object>(visitor, out var ex, out _);
            Assert.IsFalse(val);
        }
        [TestMethod]
        [DataRow("1", typeof(int))]
        [DataRow("11", typeof(long))]
        [DataRow("11", typeof(int))]
        [DataRow("1", typeof(double))]
        [DataRow("123.123", typeof(double))]
        [DataRow("1", typeof(decimal))]
        [DataRow("1.123", typeof(decimal))]
        [DataRow("dwgqiudgw", typeof(string))]
        [DataRow("1.123", typeof(float))]
        [DataRow("1", typeof(decimal))]
        [DataRow("1", typeof(decimal))]
        [DataRow("1", typeof(byte))]
        public void PasePrimary_MustPass(string value,Type type)
        {
            var visitor = new JsonVisitor(value);
            var val = JsonVisitorExtensions.TryGet(visitor, type, out var ex, out var res);
            Assert.IsTrue(val);
            Assert.IsNull(ex);
            Assert.IsInstanceOfType(res, type);
        }
        [TestMethod]
        [DataRow("1", typeof(int?))]
        [DataRow("1", typeof(double?))]
        [DataRow("1.123", typeof(double?))]
        [DataRow("1", typeof(decimal?))]
        [DataRow("1.123", typeof(decimal?))]
        [DataRow("1", typeof(byte?))]
        public void PaseNullableType_MustReturnValue(string value, Type type)
        {
            var t = Nullable.GetUnderlyingType(type);
            var visitor = new JsonVisitor(value);
            var val = JsonVisitorExtensions.TryGet(visitor, type, out var ex, out var res);
            Assert.IsTrue(val);
            Assert.IsNull(ex);
            Assert.IsInstanceOfType(res, t);
        }
        [TestMethod]
        public void ParseEnum_MustPass()
        {
            var visitor = new JsonVisitor("1");
            var val = JsonVisitorExtensions.TryGet(visitor, typeof(Colors), out var ex, out var res);
            Assert.IsTrue(val);
            Assert.IsNull(ex);
            Assert.AreEqual(Colors.Red, res);
        }
        [TestMethod]
        [DataRow("a",typeof(int))]
        [DataRow("a",typeof(int?))]
        [DataRow("a",typeof(double))]
        [DataRow("a",typeof(double?))]
        [DataRow("a",typeof(decimal))]
        [DataRow("a", typeof(decimal?))]
        [DataRow("1.123", typeof(int))]
        [DataRow("1.123", typeof(long))]
        public void ParesFail_MustOutException(string value,Type type)
        {
            var visitor = new JsonVisitor(value);
            var val = JsonVisitorExtensions.TryGet(visitor, type, out var ex, out var res);
            Assert.IsFalse(val);
            Assert.IsNotNull(ex);
        }
        [TestMethod]
        public void ParseToString_MustReturnOrigin()
        {
            var visitor = new JsonVisitor("aaa");
            var val = JsonVisitorExtensions.TryGet(visitor, typeof(string), out var ex, out var res);
            Assert.IsTrue(val);
            Assert.IsNull(ex);
            Assert.AreEqual(visitor.ToString(), res);
        }
    }
}
