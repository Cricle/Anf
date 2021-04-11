using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Easy.Test
{
    [TestClass]
    public class EasyComicBuilderTest
    {
        [TestMethod]
        public void GetTwice_DefaultMustEqual()
        {
            var a = EasyComicBuilder.Default;
            var b = EasyComicBuilder.Default;
            Assert.AreEqual(a, b);
        }
        [TestMethod]
        public void InitWithSelftService_GotMustHasValue()
        {
            var eng = new EasyComicBuilder();
            eng.Services.AddSingleton<object>();
            var prov = eng.Build();
            var val = prov.GetRequiredService<object>();
        }

        [TestMethod]
        public void Init_BuildIt_ReturnNotNull()
        {
            var eng = new EasyComicBuilder();
            eng.AddComicServices();
            eng.NetworkAdapterType = NetworkAdapterTypes.HttpClient;
            Assert.IsNotNull(eng.Services);
            var r = eng.Build();
            Assert.IsNotNull(r);
        }
    }
}
