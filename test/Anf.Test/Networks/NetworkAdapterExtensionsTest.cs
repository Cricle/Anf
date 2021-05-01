using Anf.Engine;
using Anf.Networks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test.Networks
{
    [TestClass]
    public class NetworkAdapterExtensionsTest
    {
        [TestMethod]
        public async Task GotRequest_UsingGetStringAsnyc_ReturnMustEquanWant()
        {
            var addr = "dsajifbagviufvdi2ud";
            var adapter = new NullNetworkAdapter();
            var str=await NetworkAdapterExtensions.GetStringAsync(adapter, new RequestSettings { Address = addr });
            Assert.AreEqual(addr, str);
        }
        class Student
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
        [TestMethod]
        public async Task GotRequest_UsingGetObjectAsnyc_ReturnMustBeAObject()
        {
            var stu = new Student { Name = "hello", Age = 22 };
            var addr = JsonHelper.Serialize(stu);
            var adapter = new NullNetworkAdapter();
            var str = await NetworkAdapterExtensions.GetObjectAsync<Student>(adapter, new RequestSettings { Address = addr });
            Assert.IsNotNull(str);
            Assert.AreEqual(stu.Name, str.Name);
            Assert.AreEqual(stu.Age, str.Age);
        }
    }
}
